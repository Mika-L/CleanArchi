using CleanArchi.Application.Common.Interfaces;
using CleanArchi.Application.Outbox;
using CleanArchi.Domain.Repositories;
using CleanArchi.Infrastructure.Messaging;
using CleanArchi.Infrastructure.Oubox;
using CleanArchi.Infrastructure.Persistence.EF;
using CleanArchi.Infrastructure.Persistence.EF.Interceptors;
using CleanArchi.Infrastructure.Persistence.EF.Repositories;
using CleanArchi.Web.ExceptionHandling;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;

namespace CleanArchi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddMediatR(cfg =>
            {
                //cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.RegisterServicesFromAssemblies(Assembly.Load("CleanArchi.Application"));
            });

            // EF
            builder.Services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

            builder.Services.AddDbContext<ApplicationDbContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("SQLServer"))
                    .AddInterceptors(
                        sp.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>());
                }
            );

            builder.Services.AddScoped<IUnitOfWork, UnitOfWorkEF>();
            builder.Services.AddScoped<IExpenseRepository, ExpenseRepositoryEF>();

            // Dapper
            //var cs = builder.Configuration.GetConnectionString("SQLServer");

            //builder.Services.AddScoped<UnitOfWorkDapper>(sp =>
            //    new UnitOfWorkDapper(cs));

            //builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UnitOfWorkDapper>());
            //builder.Services.AddScoped<IExpenseRepository, ExpenseRepositoryDapper>();

            // logging
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });

            // open telemetry
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                    resource.AddService(builder.Environment.ApplicationName))
                .WithTracing(tracing =>
                {
                    tracing
                        //                //.AddSource("CleanArchi.Application")
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddConsoleExporter(); // affichage des traces dans la console
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddConsoleExporter();  // affichage des métriques dans la console
                });

            // Exception handling
            builder.Services.AddProblemDetails(configure =>
                configure.CustomizeProblemDetails = options =>
                {
                    options.ProblemDetails.Extensions.TryAdd("traceId",
                        options.HttpContext.TraceIdentifier);
                }
            );
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            // messaging
            builder.Services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();

            // outbox process hosted service
            builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
            builder.Services.AddHostedService<OutboxProcessor>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseExceptionHandler();

            app.MapControllers();

            app.Run();
        }
    }
}
