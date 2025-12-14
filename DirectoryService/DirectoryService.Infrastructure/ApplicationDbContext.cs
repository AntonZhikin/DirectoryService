using DirectoryService.Domain.DepartamentLocation;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure;

public class ApplicationDbContext(string connectionString) : DbContext
{
    public DbSet<Department> Departments { get; set; }

    public DbSet<Location> Locations { get; set; }
    
    public DbSet<DepartmentLocation> DepartmentLocations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString);

        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(build => build.AddConsole()));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}