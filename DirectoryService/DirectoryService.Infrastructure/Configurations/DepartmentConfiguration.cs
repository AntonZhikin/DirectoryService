using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Path = DirectoryService.Domain.Departments.ValueObjects.Path;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                value => value.Value,
                value => new DepartmentId(value));

        builder.ComplexProperty(x => x.Name, b =>
        {
            b.Property(n => n.Value)
                .HasColumnName("name")
                .HasMaxLength(DepartmentName.NAME_MAX_LENGTH)
                .IsRequired();
        });

        builder.ComplexProperty(x => x.Identifier, b =>
        {
            b.Property(n => n.Value)
                .HasColumnName("identifier")
                .HasMaxLength(Identifier.IDENTIFIER_MAX_LENGTH)
                .IsRequired();
        });

        builder.Property(x => x.ParentId)
            .IsRequired(false)
            .HasColumnName("parent_id")
            .HasConversion(
                value => value!.Value,
                value => new DepartmentId(value));

        builder.ComplexProperty(x => x.Path, b =>
        {
            b.Property(n => n.Value)
                .HasColumnName("path")
                .IsRequired();
        });

        builder.Property(x => x.Depth)
            .IsRequired()
            .HasColumnName("depth");

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.HasMany(x => x.ChildrenDepartments)
            .WithOne(x => x.Parent)
            .IsRequired(false)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.DepartmentLocations)
            .WithOne()
            .HasForeignKey(x => x.DepartmentId);
    }
}