using DirectoryService.Shared.ErrorManagement;
using DirectoryService.Shared.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;

namespace DirectoryService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithOptions();
        services.AddOpenApiDocumentation();
        services.AddSerilogLogging(configuration);

        return services;
    }

    private static void AddControllersWithOptions(this IServiceCollection services)
    {
        services.AddControllers();
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
    }

    private static void AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer((schema, context, _) =>
            {
                if (context.JsonTypeInfo.Type == typeof(Envelope<AppError>))
                {
                    if (schema.Properties.TryGetValue("Error", out var error))
                    {
                        error.Items.Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "Error" };
                    }
                }

                return Task.CompletedTask;
            });
        });
    }

    private static void AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((serviceProvider, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(serviceProvider)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", "DirectoryService"));
    }
}
