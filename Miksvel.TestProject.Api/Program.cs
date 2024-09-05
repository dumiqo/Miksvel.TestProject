using Miksvel.TestProject.ProviderOne;
using Miksvel.TestProject.ProviderTwo;

namespace Miksvel.TestProject.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            ConfigureService(builder.Services);

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
            app.MapHealthChecks("/ping");
            app.Run();
        }

        private static void ConfigureService(IServiceCollection services)
        {
            ProviderOneDependency.Register(services);
            ProviderTwoDependency.Register(services);
        }
    }
}
