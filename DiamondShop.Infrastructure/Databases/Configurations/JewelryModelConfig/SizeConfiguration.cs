using DiamondShop.Domain.BusinessRules;
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
    internal class SizeConfiguration : IEntityTypeConfiguration<Size>
    {
        protected static List<Size> SIZES = 
            Enumerable.Range(SizeRules.MinRingSize, SizeRules.MaxRingSize)
            .Select(p => Size.Create(p, null, SizeId.Parse(p.ToString()))).ToList();
        public void Configure(EntityTypeBuilder<Size> builder)
        {
            builder.ToTable("Size");
            builder.Property(o => o.Id)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => SizeId.Parse(dbValue));
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Id);
            builder.HasData(SIZES);
        }
    }
}
