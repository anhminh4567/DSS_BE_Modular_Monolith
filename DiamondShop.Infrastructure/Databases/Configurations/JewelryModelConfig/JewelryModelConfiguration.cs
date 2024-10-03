using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.JewelryModelConfig
{
    public class JewelryModelConfiguration : IEntityTypeConfiguration<JewelryModel>
    {
        public void Configure(EntityTypeBuilder<JewelryModel> builder)
        {
            builder.ToTable("JewelryModel");
            builder.Property(o => o.Id)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => JewelryModelId.Parse(dbValue));
            builder.Property(o => o.CategoryId)
            .HasConversion(
                Id => Id.Value,
                dbValue => JewelryModelCategoryId.Parse(dbValue));
            builder.HasOne(o => o.Category).WithOne().HasForeignKey<JewelryModel>(p => p.CategoryId);
            builder.HasMany(o => o.MainDiamonds).WithOne().HasForeignKey(p => p.ModelId).IsRequired();
            builder.HasMany(o => o.SideDiamonds).WithOne().HasForeignKey(p => p.ModelId).IsRequired();
            builder.OwnsOne(o => o.Thumbnail, childBuilder =>
            {
                childBuilder.ToJson();
            });
            builder.OwnsMany(o => o.Gallery, childBuilder =>
            {
                childBuilder.ToJson();
            });
            /*builder.HasMany(o => o.Medias).WithOne().HasForeignKey(o => o.ModelId);*/
            builder.Property(o => o.Width).IsRequired(false);
            builder.Property(o => o.Length).IsRequired(false);
            builder.Property(o => o.BackType).IsRequired(false).HasConversion<string>();
            builder.Property(o => o.ClaspType).IsRequired(false).HasConversion<string>();
            builder.Property(o => o.ChainType).IsRequired(false).HasConversion<string>();
            builder.HasKey(o => o.Id);
            builder.HasIndex(o => o.Id);
        }
    }
}
