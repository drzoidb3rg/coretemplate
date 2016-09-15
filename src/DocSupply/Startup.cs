using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raven.Client;
using Raven.Client.Document;
using System.Linq;
using DocSupply.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using DocSupply.CustomErrorHandling;

namespace DocSupply
{

    public class StartupLite
    {
        public StartupLite()
        {
            
        }
    };

    //not used. Configured
    public class Startup
    {
        public bool RunningInTestRunner = false;
        public string ApplicationMode;


        public IDocumentStore GetDocumentStore()
        {

            var ravenHost = Configuration["AppSettings:RavenHost"];

            IDocumentStore store = new DocumentStore
            {
                Url = ravenHost,
                DefaultDatabase = "DocSupply"
            };

            store.Initialize();

            return store;

        }

        public Startup(IHostingEnvironment env)
        {
            //last config wins
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

            Configuration = builder.Build();
            ApplicationMode = Configuration["AppSettings:ApplicationMode"];

            if (env.EnvironmentName == "test-runner")
                RunningInTestRunner = true;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);


            if(!RunningInTestRunner)
                services.AddSingleton(GetDocumentStore());

            services.AddOptions();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddMvc();

            //services.Configure<RazorViewEngineOptions>(o => o.ViewLocationExpanders.Add(new ViewLocationRemapper()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (ApplicationMode.Equals("Test", System.StringComparison.OrdinalIgnoreCase))
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
               // app.UseCustomErrorPages();
            }

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
    }
}
