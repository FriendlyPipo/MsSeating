using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Seats.Application;
using Seats.Core.Database;
using Seats.Core.RabbitMQ;
using Seats.Core.Repositories;
using Seats.Infrastructure.Database;
using Seats.Infrastructure.Database.Context;
using Seats.Core.Exceptions;
using Seats.Infrastructure.RabbitMQ.Connection;
using Seats.Infrastructure.RabbitMQ.Consumer;
using Seats.Infrastructure.RabbitMQ.Producer;
using Seats.Infrastructure.Repositories;

using Seats.Core.Services;
using Seats.Infrastructure.Services;

namespace Seats.Api
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGenWithAuth(configuration);
            services.KeycloakConfiguration(configuration);
            
            // Allow CORS
             services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddHttpClient<ITokenService, TokenService>();

            services.AddHttpClient<IUserLogService, UserLogService>();
            services.AddHttpClient<IUserAuditService, UserAuditService>();
            services.AddHttpClient<IEventService, EventService>();

            services.AddApplication();

            var connectionString = configuration.GetConnectionString("PostgresSQLConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ConfigurationException("LEl parametro de conexion esta vacio");
            }

            services.AddDbContextFactory<SeatsDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<SeatsDbContext>(provider => 
                provider.GetRequiredService<IDbContextFactory<SeatsDbContext>>().CreateDbContext());

            services.AddScoped<ISeatsDbContext>(provider => provider.GetRequiredService<SeatsDbContext>());
            services.AddScoped<ISeatsDbContextTransactionProxy, SeatsDbContextTransactionProxy>();

            // Repositories
            services.AddScoped<ISeatRepository, SeatRepository>();

            // RabbitMQ Connection
            services.AddSingleton<IConnectionFactory>(_ =>
            {
                var rabbitMqSection = configuration.GetSection("RabbitMQ");
                var hostName = rabbitMqSection["HostName"];
                var port = int.TryParse(rabbitMqSection["Port"], out var p) ? p : 15672;
                var userName = rabbitMqSection["UserName"];
                var password = rabbitMqSection["Password"];

                return new ConnectionFactory
                {
                    HostName = hostName,
                    Port = port,
                    UserName = userName,
                    Password = password
                };
            });

            services.AddSingleton<IConnectionRabbitMQ>(provider =>
            {
                var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
                var rabbitMQConnection = new RabbitMQConnection(connectionFactory);
                rabbitMQConnection.InitializeAsync().GetAwaiter().GetResult();
                return rabbitMQConnection;
            });

            // Producer
            services.AddScoped(typeof(IEventBus<>), typeof(RabbitMQProducer<>));

            // Consumers
            services.AddSingleton<CreateSeatConsumer>();
            services.AddSingleton<DeleteSeatConsumer>();
            services.AddSingleton<UpdateSeatStatusConsumer>();

            services.AddSingleton<IRabbitMQConsumer>(sp => new CompositeRabbitMQConsumer(new IRabbitMQConsumer[]
            {
                sp.GetRequiredService<CreateSeatConsumer>(),
                sp.GetRequiredService<DeleteSeatConsumer>(),
                sp.GetRequiredService<UpdateSeatStatusConsumer>()
            }));

            services.AddHostedService<RabbitMQBackgroundService>();

            return services;
        }
    }
}
