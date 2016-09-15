using System;
using System.Collections.Generic;
using System.IO;
using DocSupply.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client;
using Raven.Client.Document;
using DocSupply.CustomErrorHandling;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace DocSupply
{
    public class ApplicationHostConfig
    {
        public IHostingEnvironment HostingEnvironment;
        public IConfigurationRoot ConfigurationRoot;
        public string ApplicationMode;

        public Action<IServiceCollection> AddRavenAction;

        public Action<IServiceCollection> ConfigureServicesAction;

        public Func<IHostingEnvironment> HostingEnvironmentFunc;

        public Func<IHostingEnvironment, IConfigurationRoot> ConfigFunc;

        public List<Action<IApplicationBuilder, IConfigurationRoot>> ConfigureFirst;

        public Action<IApplicationBuilder, IConfigurationRoot> ConfigureLiveAction;

        public Action<IApplicationBuilder, IConfigurationRoot> ConfigureTestAction;

        public Action<IApplicationBuilder, IConfigurationRoot> ConfigureForModeAction;

        public Action<IApplicationBuilder, IConfigurationRoot> ConfigureCommonAction;

        public Action<IApplicationBuilder> ConfigureAction;

        public Action<ILoggerFactory> ConfigureLoggingAction;

        public Func<string> ContentRootPath;

        public ApplicationHostConfig()
        {
            ConfigureFirst = new List<Action<IApplicationBuilder, IConfigurationRoot>>();
            AddRavenAction = (sc) => DoRaven(sc, ConfigurationRoot);
            HostingEnvironmentFunc = () => GetHostingEnvironment(ContentRootPath);
            ConfigFunc = GetConfig;
            ConfigureServicesAction = (sc) => ConfigureServices(sc, ConfigurationRoot);
            ConfigureLiveAction = ConfigureLive;
            ConfigureTestAction = ConfigureTest;
            ConfigureCommonAction = ConfigureCommon;
            ContentRootPath = Directory.GetCurrentDirectory;
            ConfigureLoggingAction = ConfigureLogging;
        }


        public ApplicationHostConfig Initialise()
        {
            HostingEnvironment = HostingEnvironmentFunc();
            ConfigurationRoot = ConfigFunc(HostingEnvironment);

            ApplicationMode = ConfigurationRoot["AppSettings:ApplicationMode"];
            ConfigureAction = (sc) => ConfigureAll(sc, ConfigurationRoot);

            return this;
        }

        private static void ConfigureCommon(IApplicationBuilder app, IConfigurationRoot config)
        {
            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "ds",
                LoginPath = new PathString("/Signin/"),
                AccessDeniedPath = new PathString("/Signin/Forbidden/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        private void ConfigureAll(IApplicationBuilder app, IConfigurationRoot config)
        {
            ConfigureFirst.ForEach(a => a(app,config));

            if (ApplicationMode.Equals("Test", StringComparison.OrdinalIgnoreCase))
                ConfigureTestAction(app, config);

            if (ApplicationMode.Equals("Live", StringComparison.OrdinalIgnoreCase))
                ConfigureLiveAction(app, config);

            ConfigureCommonAction(app, config);
        }

        private static void ConfigureTest(IApplicationBuilder app, IConfigurationRoot config)
        {
            app.UseTestErrorHandling();
            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();  
        }

        private static void ConfigureLive(IApplicationBuilder app, IConfigurationRoot config)
        {
            app.UseLiveErrorHandling();
        }

        private static IConfigurationRoot GetConfig(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            var config = builder.Build();
            return config;
        }

        private static IHostingEnvironment GetHostingEnvironment(Func<string> contentRootPath)
        {
            var hostingEnvironment = new HostingEnvironment();

            var webHostOptions = new WebHostOptions();

            hostingEnvironment.Initialize("DocSupply", contentRootPath(), webHostOptions);

            hostingEnvironment.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            return hostingEnvironment;
        }


        private static void ConfigureServices(IServiceCollection services, IConfigurationRoot config)
        {
            services.AddApplicationInsightsTelemetry(config);

            services.AddOptions();

            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            services.AddMvc();


            services.AddAuthorization(options =>
            {
                options.AddPolicy("SignedIn", policy => policy.RequireClaim("Id"));
            });

            services.Configure<BuildVersion>(options =>
            {
                options.BuildNumber = GetBuildNumber();
            });

            services.Configure<RazorViewEngineOptions>(o => o.ViewLocationExpanders.Add(new ViewLocationRemapper()));
        }

        private static string GetBuildNumber()
        {
            var path = "releaseBuild.txt";
            var buildNumber = "";
            if (!File.Exists(path))
                return "Build version file not found";

            using (StreamReader reader = File.OpenText(path))
            {
                buildNumber = reader.ReadLine();
            }
            return buildNumber;
        }

        private static void DoRaven(IServiceCollection services, IConfigurationRoot config)
        {
            services.AddSingleton(GetDocumentStore(config));
        }

        private static IDocumentStore GetDocumentStore(IConfigurationRoot config)
        {
            var ravenHost = config["AppSettings:RavenHost"];

            IDocumentStore store = new DocumentStore
            {
                Url = ravenHost,
                DefaultDatabase = "DocSupply"
            };

            store.Initialize();

            try
            {

                //need to work out how to get reference to assembly, so we can scan and load all indexes
                //IndexCreation.CreateIndexes(typeof(Startup).Assembly, store);

                //explicitly add each index for now
                new Features.UserAccount.UsersByAthensPuid().Execute(store);
                new Features.DiscoveryJob.DiscoveryJobRecordsByUser().Execute(store);
            }
            catch (Exception ex)
            {
                // log here, raven is down? 
            }


            return store;
        }

        private void ConfigureLogging(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(ConfigurationRoot.GetSection("Logging"));
            loggerFactory.AddDebug();
        }
    }
}