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
        protected static List<Size> SIZES = new List<Size>
        {
            Size.Create(3,null, SizeId.Parse(1.ToString())),
            Size.Create(4,null, SizeId.Parse(2.ToString())),
            Size.Create(5,null, SizeId.Parse(3.ToString())),
            Size.Create(6,null, SizeId.Parse(4.ToString())),
            Size.Create(7,null, SizeId.Parse(5.ToString())),
            Size.Create(8,null, SizeId.Parse(6.ToString())),
            Size.Create(9,null, SizeId.Parse(7.ToString())),
            Size.Create(10,null, SizeId.Parse(8.ToString())),
            Size.Create(11,null, SizeId.Parse(9.ToString())),
            Size.Create(12,null, SizeId.Parse(10.ToString()))
        };
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
