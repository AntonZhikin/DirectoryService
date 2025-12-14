using DirectoryService.Domain.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(x => x.Id);

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

        builder.OwnsMany(x => x.Addresses, ab => { ab.ToJson("addresses"); });

        builder.ComplexProperty(x => x.Timezone, nb =>
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