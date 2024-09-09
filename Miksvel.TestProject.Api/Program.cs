using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Miksvel.TestProject.Api.Services;
using Miksvel.TestProject.ProviderOne;
using Miksvel.TestProject.ProviderTwo;

namespace Miksvel.TestProject.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                     .AddEnvironmentVariables()
                                     .AddCommandLine(args)
                                     .AddJsonFile("appsettings.json")
                                     .Build();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            ConfigureService(builder.Services, configuration);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();
            app.MapHealthChecks("/ping", new HealthCheckOptions
            {
                ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status500InternalServerError
                    }
            });
            app.Run();
        }

        private static void ConfigureService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAggregateSearchService, AggregateSearchService>();

            ProviderOneDependency.Register(services, configuration);
            ProviderTwoDependency.Register(services, configuration);
        }
    }
}
