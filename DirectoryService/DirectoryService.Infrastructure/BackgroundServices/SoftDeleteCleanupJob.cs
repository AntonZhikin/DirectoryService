using DirectoryService.Infrastructure.Database;
using DirectoryService.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace DirectoryService.Infrastructure.BackgroundServices;

[DisallowConcurrentExecution]
public class SoftDeleteCleanupJob(
    SoftDeleteCleaner cleaner,
    IOptions<SoftDeleteCleanupOptions> options,
    ILogger<SoftDeleteCleanupJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var settings = options.Value;

        try
        {
            var deletedBefore = DateTime.UtcNow - settings.RetentionPeriod;

            int deleted = await cleaner.CleanupAsync(deletedBefore, settings.BatchSize, context.CancellationToken);

            logger.LogInformation(
                "Soft delete cleanup finished: {DeletedCount} records deleted before {DeletedBefore}",
                deleted,
                deletedBefore);
        }
        catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Soft delete cleanup was cancelled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Soft delete cleanup failed");
        }
    }
}
