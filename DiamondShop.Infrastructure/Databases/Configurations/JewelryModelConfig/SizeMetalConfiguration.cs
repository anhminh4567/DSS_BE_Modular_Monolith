using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiamondShop.Infrastructure.Databases.Configurations.JewelryModelConfig
{
    internal class SizeMetalConfiguration : IEntityTypeConfiguration<SizeMetal>
    {
        public void Configure(EntityTypeBuilder<SizeMetal> builder)
        {
            builder.ToTable("SizeMetal");
            builder.Property(o => o.SizeId)
            .HasConversion(
                Id => Id.Value,
                dbValue => SizeId.Parse(dbValue));
            builder.Property(o => o.MetalId)
            .HasConversion(
                Id => Id.Value,
                dbValue => MetalId.Parse(dbValue));
            builder.Property(o => o.ModelId)
            .HasConversion(
                Id => Id.Value,
                dbValue => JewelryModelId.Parse(dbValue));
            builder.HasOne(o => o.Size).WithMany().HasForeignKey(o => o.SizeId).IsRequired();
            builder.HasOne(o => o.Metal).WithMany().HasForeignKey(o => o.MetalId).IsRequired();
            builder.HasOne(o => o.Model).WithMany(p => p.SizeMetals).HasForeignKey(o => o.ModelId).IsRequired();
            builder.HasKey(o => new { o.SizeId, o.MetalId, o.ModelId });
        }
    }
}
