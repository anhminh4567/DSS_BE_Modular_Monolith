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
        public void Configure(EntityTypeBuilder<JewelryModelCategory> builder)
        {
            builder.ToTable("JewelryModelCategory");
            builder.Property(o => o.Id)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => JewelryModelCategoryId.Parse(dbValue));
            builder.HasOne(o => o.ParentCategory).WithOne().HasForeignKey<JewelryModelCategory>(o => o.ParentCategoryId).IsRequired(false);
            builder.HasKey(o => o.Id);
        }
    }
}
