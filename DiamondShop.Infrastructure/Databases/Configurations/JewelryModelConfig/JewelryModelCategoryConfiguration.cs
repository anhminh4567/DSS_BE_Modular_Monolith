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
    internal class JewelryModelCategoryConfiguration : IEntityTypeConfiguration<JewelryModelCategory>
    {
        private static List<JewelryModelCategory> CATEGORIES = new()
        {
            JewelryModelCategory.Create("Ring", "A normal ring", "", true, null, JewelryModelCategoryId.Parse("1")),
            JewelryModelCategory.Create("Necklace", "A normal necklace", "", true, null, JewelryModelCategoryId.Parse("2")),
            JewelryModelCategory.Create("Bracelace", "A normal bracelace", "", true, null, JewelryModelCategoryId.Parse("3")),
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
