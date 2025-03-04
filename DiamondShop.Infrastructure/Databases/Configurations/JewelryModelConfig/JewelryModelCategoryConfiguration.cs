﻿using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiamondShop.Infrastructure.Databases.Configurations.JewelryModelConfig
{
    internal class JewelryModelCategoryConfiguration : IEntityTypeConfiguration<JewelryModelCategory>
    {
        public List<JewelryModelCategory> CATEGORIES = new()
        {
            JewelryModelCategory.Create("Ring", "A normal ring", "", true, null, JewelryModelCategoryId.Parse("1")),
            JewelryModelCategory.Create("Necklace", "A normal necklace", "", true, null, JewelryModelCategoryId.Parse("2")),
            JewelryModelCategory.Create("Bracelet", "A normal bracelet", "", true, null, JewelryModelCategoryId.Parse("3")),
            JewelryModelCategory.Create("Earring", "A normal earring", "", true, null, JewelryModelCategoryId.Parse("4")),
        };
        public void Configure(EntityTypeBuilder<JewelryModelCategory> builder)
        {
            builder.ToTable("JewelryModelCategory");
            builder.Property(o => o.Id)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => JewelryModelCategoryId.Parse(dbValue));
            builder.Property(o => o.ParentCategoryId)
            .HasConversion(
                Id => Id.Value,
                dbValue => JewelryModelCategoryId.Parse(dbValue));
            builder.HasOne(o => o.ParentCategory).WithOne().HasForeignKey<JewelryModelCategory>(o => o.ParentCategoryId).IsRequired(false);
            builder.HasKey(o => o.Id);
            builder.HasData(CATEGORIES);
        }
    }
}
