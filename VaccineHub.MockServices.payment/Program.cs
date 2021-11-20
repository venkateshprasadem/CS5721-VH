using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VaccineHub.MockServices.payment
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseUrls(new []{"http://localhost:5010"})
                .ConfigureLogging(i => { i.AddConsole(); })
                .UseKestrel()
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}