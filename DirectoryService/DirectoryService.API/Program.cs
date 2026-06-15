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
using FluentValidation;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq") ?? throw new Exception("Not found connection string Seq"))
    .CreateLogger();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

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

builder.Services.AddSerilog();

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
