using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

/// <summary>
/// Entity Framework Core configuration for Sale entity
/// </summary>
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    /// <summary>
    /// Configures the Sale entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SaleNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.SaleDate)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.CustomerId)
            .IsRequired();

        builder.Property(s => s.CustomerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.BranchId)
            .IsRequired();

        builder.Property(s => s.BranchName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.IsCancelled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(s => s.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // Configure relationship with SaleItems
        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Add index for SaleNumber for faster lookups
        builder.HasIndex(s => s.SaleNumber)
            .IsUnique();

        // Add indexes for common query patterns
        builder.HasIndex(s => s.CustomerId);
        builder.HasIndex(s => s.BranchId);
        builder.HasIndex(s => s.IsCancelled);
        builder.HasIndex(s => s.SaleDate);
    }
}