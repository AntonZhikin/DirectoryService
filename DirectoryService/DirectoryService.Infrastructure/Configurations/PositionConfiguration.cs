using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("position");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                value => value.Value,
                value => new PositionId(value));

        builder.ComplexProperty(x => x.Name, bm =>
        {
            bm.Property(n => n.Value)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(PositionName.MAX_LENGTH_POSITION_NAME);
        });
        
        builder.ComplexProperty(x => x.Description, bm =>
        {
            bm.Property(n => n.Value)
                .IsRequired()
                .HasColumnName("description")
                .HasMaxLength(PositionDescription.MAX_LENGTH_POSITION_DESCRIPTION);
        });
        
        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}