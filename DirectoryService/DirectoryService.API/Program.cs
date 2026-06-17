using System.Globalization;
using DirectoryService.API.Middlewares;
using DirectoryService.Application.Database;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Application.Departments.Linking;
using DirectoryService.Application.Departments.Unlinking;
using DirectoryService.Application.Departments.Update;
using DirectoryService.Application.Locations.Create;
using DirectoryService.Application.Locations.Update;
using DirectoryService.Application.Validation;
using DirectoryService.Infrastructure;
using DirectoryService.Infrastructure.Database;
using DirectoryService.Infrastructure.Repositories.Locations;
using DirectoryService.Infrastructure.Repositories.Departments;
using DirectoryService.Shared.ErrorManagement;
using DirectoryService.Shared.Response;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .WriteTo.Debug()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq") ?? throw new Exception("Not found connection string Seq"))
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application...");
    
    builder.Services.AddControllers();
    builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

    builder.Services.AddOpenApi(options =>
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

    builder.Services.AddScoped<ApplicationDbContext>(_ =>
        new ApplicationDbContext(builder.Configuration.GetConnectionString("DatabaseConnection")!));

    builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

    string? repositoryProvider = builder.Configuration["RepositoryProvider"];

    if (repositoryProvider == "Dapper")
        builder.Services.AddScoped<ILocationRepository, DapperLocationRepository>();
    else
        builder.Services.AddScoped<ILocationRepository, EfCoreLocationRepository>();

    builder.Services.AddScoped<IDepartmentRepository, EfCoreDepartmentRepository>();

    builder.Services.AddScoped<CreateDepartmentHandler>();
    builder.Services.AddScoped<CreateLocationHandler>();
    builder.Services.AddScoped<UpdateDepartmentHandler>();
    builder.Services.AddScoped<UpdateLocationHandler>();
    builder.Services.AddScoped<LinkingLocationHandler>();
    builder.Services.AddScoped<UnlinkingLocationHandler>();

    builder.Services.AddValidatorsFromAssembly(typeof(CustomValidators).Assembly);

    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("ServiceName", "DirectoryService")
    );

    var app = builder.Build();

    app.UseExceptionMiddleware();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseSerilogRequestLogging();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


