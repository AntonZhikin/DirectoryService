using DirectoryService.Domain.Location;
using DirectoryService.Domain.Location.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.Property(x => x.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                value => value.Value,
                value => new LocationId(value));

        builder.ComplexProperty(x => x.Name, nb =>
        {
            nb.Property(n => n.Value)
                .HasColumnName("name")
                .HasMaxLength(LocationName.NAME_MAX_LENGHT)
                .IsRequired();
        });

        builder.ComplexProperty(x => x.Address, nb =>
        {
            nb.Property(n => n.City)
                .HasColumnName("city")
                .IsRequired();
            nb.Property(n => n.Street)
                .HasColumnName("street")
                .IsRequired();
            nb.Property(n => n.HouseNumber)
                .HasColumnName("house_number")
                .IsRequired();
            nb.Property(n => n.Number)
                .HasColumnName("number")
                .IsRequired();
        });

        builder.ComplexProperty(x => x.TimeZone, nb =>
        {
            nb.Property(n => n.Value)
                .HasColumnName("timezone")
                .IsRequired();
        });

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.HasMany(x => x.DepartmentLocations)
            .WithOne()
            .HasForeignKey(x => x.LocationId);
    }
}