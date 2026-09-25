using DirectoryService.Application.Database;
using DirectoryService.Application.Database.Repository;
using DirectoryService.Application.Database.Transaction;
using DirectoryService.Infrastructure.BackgroundServices;
using DirectoryService.Infrastructure.Database;
using DirectoryService.Infrastructure.Models;
using DirectoryService.Infrastructure.Repositories.Departments;
using DirectoryService.Infrastructure.Repositories.Locations;
using DirectoryService.Infrastructure.Repositories.Positions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddBackgroundJobs(configuration);

        return services;
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ApplicationDbContext>(_ =>
            new ApplicationDbContext(configuration.GetConnectionString("DatabaseConnection")!));

        services.AddScoped<IReadDbContext, ApplicationDbContext>(_ =>
            new ApplicationDbContext(configuration.GetConnectionString("DatabaseConnection")!));

        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddScoped<ITransactionManager, TransactionManager>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
    }

    private static void AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SoftDeleteCleanupOptions.SECTION);
        var settings = section.Get<SoftDeleteCleanupOptions>() ?? new SoftDeleteCleanupOptions();

        services.AddOptions<SoftDeleteCleanupOptions>()
            .Bind(section)
            .Validate(
                o => CronExpression.IsValidExpression(o.Cron) && o.BatchSize > 0 && o.RetentionPeriod > TimeSpan.Zero,
                "SoftDeleteCleanup: Cron must be valid, BatchSize and RetentionPeriod must be positive")
            .ValidateOnStart();

        services.AddSingleton<SoftDeleteCleaner>();

        if (!settings.Enabled)
            return;

        services.AddQuartz(quartz =>
        {
            var jobKey = new JobKey(nameof(SoftDeleteCleanupJob));

            quartz.AddJob<SoftDeleteCleanupJob>(job => job.WithIdentity(jobKey));

            quartz.AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithIdentity($"{nameof(SoftDeleteCleanupJob)}-trigger")
                .WithCronSchedule(settings.Cron));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }
}
