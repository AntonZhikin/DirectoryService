using DirectoryService.Application;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Validation;
using DirectoryService.Infrastructure;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<ApplicationDbContext>(_ =>
    new ApplicationDbContext(builder.Configuration.GetConnectionString("DatabaseConnection")!));

builder.Services.AddOpenApi();

builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<CreateLocationHandler>();

builder.Services.AddValidatorsFromAssembly(typeof(CustomValidators).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.MapControllers();

app.Run();