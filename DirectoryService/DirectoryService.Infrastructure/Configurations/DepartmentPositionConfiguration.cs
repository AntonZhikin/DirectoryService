using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_positions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                value => value.Value,
                value => new DepartmentPositionId(value));

        builder.Property(x => x.DepartmentId)
            .IsRequired()
            .HasColumnName("department_id")
            .HasConversion(
                value => value.Value,
                value => new DepartmentId(value));

        builder.Property(x => x.PositionId)
            .IsRequired()
            .HasColumnName("location_id")
            .HasConversion(
                value => value.Value,
                value => new PositionId(value));
    }
}