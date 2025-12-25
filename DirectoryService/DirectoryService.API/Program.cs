using DirectoryService.API.Middlewares;
using DirectoryService.Application;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Validation;
using DirectoryService.Infrastructure;
using FluentValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq") ?? throw new Exception("Not found connection string Seq"))
    .CreateLogger();

builder.Services.AddControllers();

builder.Services.AddScoped<ApplicationDbContext>(_ =>
    new ApplicationDbContext(builder.Configuration.GetConnectionString("DatabaseConnection")!));

builder.Services.AddOpenApi();

builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<CreateLocationHandler>();

builder.Services.AddValidatorsFromAssembly(typeof(CustomValidators).Assembly);

builder.Services.AddSerilog();

var app = builder.Build();

app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();