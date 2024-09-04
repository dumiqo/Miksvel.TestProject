using Microsoft.Extensions.Hosting;

namespace Miksvel.TestProject.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                     .AddEnvironmentVariables()
                                     .AddCommandLine(args)
                                     .AddJsonFile("appsettings.json")
                                     .Build();

            var builder = CreateHostBuilder(args, configuration);
            using var host = builder.Build();

            await host.RunAsync();
        }

        public static HostApplicationBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<HostedService>();

            builder.Logging.AddConsole();
            return builder;
        }
    }
    public class HostedService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
