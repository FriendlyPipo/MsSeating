using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Seats.Infrastructure.Database.Context;

namespace Seats.Infrastructure.Database
{
    // Enables EF Core tooling to create the context at design time.
    public class SeatsDbContextFactory : IDesignTimeDbContextFactory<SeatsDbContext>
    {
        public SeatsDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = "Development";
            }

            // Try loading settings from both the working directory and the API project.
            var basePath = Directory.GetCurrentDirectory();
            var apiPath = Path.Combine(basePath, "..", "Seats.Api");

            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            AddJsonIfExists(builder, Path.Combine(basePath, "appsettings.json"));
            AddJsonIfExists(builder, Path.Combine(basePath, $"appsettings.{environment}.json"));
            AddJsonIfExists(builder, Path.Combine(apiPath, "appsettings.json"));
            AddJsonIfExists(builder, Path.Combine(apiPath, $"appsettings.{environment}.json"));

            var configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("PostgresSQLConnection")
                                   ?? configuration["ConnectionStrings:PostgresSQLConnection"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'PostgresSQLConnection' was not found. " +
                    "Ensure appsettings.json is present or set ConnectionStrings__PostgresSQLConnection env var.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<SeatsDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new SeatsDbContext(optionsBuilder.Options);
        }

        private static void AddJsonIfExists(IConfigurationBuilder builder, string path)
        {
            if (File.Exists(path))
            {
                builder.AddJsonFile(path, optional: true, reloadOnChange: false);
            }
        }
    }
}
