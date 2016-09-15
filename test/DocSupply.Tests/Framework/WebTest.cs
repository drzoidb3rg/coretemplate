using NUnit.Framework;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using DocSupply.Models;
using System;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client;
using Raven.Client.Indexes;
using System.Collections.Generic;
using System.IO;
using System.Net;
using DocSupply.Tests.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Raven.Client.Document;
using Shouldly;
using Shouldly.Configuration;

namespace DocSupply.Tests
{
    public class WebTest : IDisposable
    {
        private IDocumentStore _store;

        public HttpResponseMessage Response;

        public List<Action<IDocumentStore>> StoreActions = new List<Action<IDocumentStore>> { r => { } };

        public List<Action<HttpResponseMessage>> ResponseActions = new List<Action<HttpResponseMessage>> { r => { } };

        public List<Action<RequestBuilder>> RequestBuilderActions = new List<Action<RequestBuilder>> { r => { } };

        public Action<ShouldMatchConfigurationBuilder> WithApprovalFolder = builder => builder.SubFolder("Approvals");


        public WebTest()
        {
            WithEnvironmentVariable("AppSettings:ApplicationMode", "Live");
            WithRequestHeader("No-Layout", "true");
        }

        public static void WithDiffTool()
        {
            try
            {
                var diffTool = ShouldlyConfiguration.DiffTools.GetDiffTool();
            }
            catch (Exception)
            {
                Console.WriteLine("Could not find diff tool, registering vsDiffMerge.exe");

                var argGen = new DiffTool.ArgumentGenerator((received, approved, exists) => "\"{recieved}\" \"{approved}\" /t");

                var dir = Directory.GetCurrentDirectory();

                var location = dir + @"\Difftools\vsDiffMerge.exe";

                var vsdiff = new DiffTool("vsDiffMerge", location, argGen);

                ShouldlyConfiguration.DiffTools.RegisterDiffTool(vsdiff);


                ShouldlyConfiguration.DiffTools.AddDoNotLaunchStrategy(new DoNotLaunchWhenEnvVariableIsPresent("BUILD_SERVER"));
            }
        }

        private void SetResponse(HttpResponseMessage response)
        {
            Response = response;

            ResponseActions.ForEach(x => x(response));
        }

        public static IDocumentStore GetDocumentStore()
        {
            var store = new Raven.Client.Embedded.EmbeddableDocumentStore
            {
                Configuration =
                {
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    RunInMemory = true,
                },
                Conventions = new DocumentConvention
                {
                    //AlwaysWaitForNonStaleResultsAsOfLastWrite should only be used for tests
                    DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite,
                },
                DefaultDatabase = "testrunner"
            };
            store.Initialize();


            return store;
        }

        public TestServer GetServer(IDocumentStore documentStore)
        {
            if (documentStore == null)
                documentStore = GetDocumentStore();

            StoreActions.ForEach(a => a(documentStore));

            var directory = Directory.GetCurrentDirectory();
            var setDir = Path.GetFullPath(Path.Combine(directory, "..", "..", "src/docsupply"));

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            var appHost = new ApplicationHostConfig
            {
                AddRavenAction = (sc) => { sc.AddSingleton(documentStore); },
                ContentRootPath = () => setDir,
            };
            
            //inject the test authencication middleware
            appHost.ConfigureFirst.Add((a, c) =>{a.UseMiddleware<AuthenticatedTestRequestMiddleware>();});

            appHost.Initialise();

            var name = new StartupLite().GetType().Assembly.GetName().Name;

            var host = new WebHostBuilder()
                .UseContentRoot(setDir)
                .UseConfiguration(appHost.ConfigurationRoot)
                .ConfigureServices((sc => appHost.AddRavenAction(sc)))
                .ConfigureServices((sc => appHost.ConfigureServicesAction(sc)))
                .Configure(ap => appHost.ConfigureAction(ap))
                .UseSetting(WebHostDefaults.ApplicationKey, name);

            var server = new TestServer(host);

            return server;
        }

        public WebTest EnsureSuccessStatusCode()
        {
            ResponseActions.Add(r => r.EnsureSuccessStatusCode());

            return this;
        }

        public WebTest WithEnvironmentVariable(string key, string value)
        {
            Environment.SetEnvironmentVariable(key, value);
            return this;
        }

        public WebTest WithStore(IDocumentStore store)
        {
            _store = store;
            return this;
        }

        public WebTest WithStoreAction(Action<IDocumentStore> action)
        {
            StoreActions.Add(action);
            return this;
        }

        public WebTest WithIndex(AbstractIndexCreationTask index) 
        {
            index.Execute(_store);

            return this;
        }

        public WebTest AsJson()
        {
            WithRequestHeader("Accept", "application/json");
            return this;
        }

        public WebTest AsHtml()
        {
            WithRequestHeader("Accept", "text/html");
            return this;
        }

        public WebTest WithRequestHeader(string key, string value)
        {
            RequestBuilderActions.Add(rb => { rb.AddHeader(key, value); });
            return this;
        }

        private RequestBuilder GetRequestBuider(string requestUri)
        {
            var rb = GetServer(_store).CreateRequest(requestUri);

            RequestBuilderActions.ForEach(a => a(rb));

            return rb;
        }

        public WebTest GetOK(string requestUri)
        {
            return Get(requestUri).IsOk();
        }

        public WebTest GetInternalServerError(string requestUrl)
        {
            return Get(requestUrl);
        }

        public WebTest PostFile(string requestUri, string filePath, string fileName)
        {
            var bytes = File.ReadAllBytes(filePath);
            return PostFile(requestUri, bytes, fileName);
        }

        public WebTest Post(string requestUri, HttpContent content)
        {
            Response = GetRequestBuider(requestUri)
                       .And(x => x.Content = content)
                       .PostAsync().Result;

            return this;
        }


        public WebTest PostFile(string requestUri, byte[] bytes, string fileName)
        {
            HttpContent fileStreamContent = new StreamContent(new MemoryStream(bytes));

            var multi = new MultipartFormDataContent {{fileStreamContent, fileName, fileName}};

            return Post(requestUri, multi);

        }

        public WebTest Get(string requestUri)
        {
            Response = GetRequestBuider(requestUri).GetAsync().Result;

            return this;
        }

        public WebTest Post(string requestUri, object data)
        {
            return Post(requestUri, data.ToHttpStringContent());
        }

        public WebTest Put(string requestUri, HttpContent content)
        {
            Response = GetRequestBuider(requestUri)
                        .And(x => x.Content = content)
                        .SendAsync("PUT").Result;

            return this;
        }


        public T GetFromStore<T>(string id) where T : RavenRecord
        {
            return _store.Load<T>(id);
        }

        public WebTest IsRedirect() { return IsStatusCode(HttpStatusCode.Redirect); }

        public WebTest IsFound() { return IsStatusCode(HttpStatusCode.Found); }

        public WebTest IsOk() { return IsStatusCode(HttpStatusCode.OK);}

        public WebTest IsBadRequest() { return IsStatusCode(HttpStatusCode.BadRequest); }

        public WebTest IsNotFound() { return IsStatusCode(HttpStatusCode.NotFound); }

        public WebTest IsInternalServerError() { return IsStatusCode(HttpStatusCode.InternalServerError); }

        public WebTest IsStatusCode(HttpStatusCode code)
        {
            Assert.AreEqual(code, Response.StatusCode);
            return this;
        }

        public WebTest ResponseContains(string str)
        {
            Assert.IsTrue(ResponseString().Contains(str));
            return this;
        }

        public string ResponseString()
        {
            return Response.Content.ReadAsStringAsync().Result;
        }

        public string PrettyJsonResponse()
        {
            return Response.Content.ReadAsStringAsync().Result.JsonPrettify();
        }


        public Task<string> GetResponseStringAsync()
        {
            return Response.Content.ReadAsStringAsync();
        }

        public WebTest ResponseIsHtml()
        {
            var headers = Response.Content.Headers.ToString();
            Assert.IsTrue(headers.Contains("text/html"));
            return this;
        }

        public WebTest ResponseIsNotHtml()
        {
            var headers = Response.Content.Headers.ToString();
            Assert.IsFalse(headers.Contains("text/html"));
            return this;
        }

        public WebTest IsHtmlContent()
        {
            Assert.AreEqual("text/html", Response.Content.Headers.ContentType.MediaType);
            return this;
        }

        public WebTest IsJsonContent()
        {
            Assert.AreEqual("application/json", Response.Content.Headers.ContentType.MediaType);
            return this;
        }

        public void Dispose()
        {

        }

    }


}
