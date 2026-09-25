namespace DirectoryService.Infrastructure.Models;

public class SoftDeleteCleanupOptions
{
    public const string SECTION = "SoftDeleteCleanup";

    public bool Enabled { get; set; } = true;

    public string Cron { get; set; } = "0 0 3 * * ?";

    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(30);

    public int BatchSize { get; set; } = 500;
}
