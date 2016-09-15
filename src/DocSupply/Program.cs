using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DocSupply
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var appHost = new ApplicationHostConfig().Initialise();

            var host = new WebHostBuilder()
             .UseUrls("http://+:80")
             .UseKestrel()
             .UseIISIntegration()
             .ConfigureLogging(appHost.ConfigureLoggingAction)
             .UseConfiguration(appHost.ConfigurationRoot)
             .ConfigureServices((sc => appHost.AddRavenAction(sc)))
             .ConfigureServices((sc => appHost.ConfigureServicesAction(sc)))
             .Configure(ap => appHost.ConfigureAction(ap))
             .Build();

             host.Run();

            //var host = new WebHostBuilder()
            //    .UseUrls("http://+:80")
            //    .UseKestrel()
            //    .UseContentRoot(Directory.GetCurrentDirectory())
            //    .UseIISIntegration()
            //    .UseStartup<Startup>()
            //    .Build();

            //host.Run();

        }
    }
}
