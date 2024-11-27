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
    internal class MetalConfiguration : IEntityTypeConfiguration<Metal>
    {
        //Price per gram
        protected static List<Metal> METALS = new List<Metal>
        {
            Metal.Create("Platinum", "Bạch kim", 778_370, MetalId.Parse(1.ToString())),
            Metal.Create("14K Yellow Gold", "Vàng 14K", 1_217_096, MetalId.Parse(2.ToString())),
            Metal.Create("14K White Gold", "Vàng trắng 14k", 1_217_096, MetalId.Parse(3.ToString())),
            Metal.Create("14K Pink Gold", "Vàng hồng 14K", 1_217_096, MetalId.Parse(4.ToString())),
            Metal.Create("16K Yellow Gold", "Vàng 16K",1_391_318, MetalId.Parse(5.ToString())),
            Metal.Create("16K White Gold", "Vàng trắng 16K",1_391_318, MetalId.Parse(6.ToString())),
            Metal.Create("16K Pink Gold", "Vàng hồng 16K",1_391_318, MetalId.Parse(7.ToString())),
            Metal.Create("18K Yellow Gold", "Vàng 18K",1_565_233, MetalId.Parse(8.ToString())),
            Metal.Create("18K White Gold", "Vàng trắng 18K",1_565_233, MetalId.Parse(9.ToString())),
            Metal.Create("18K Pink Gold", "Vàng hồng 18K",1_565_233, MetalId.Parse(10.ToString())),
        };
        public void Configure(EntityTypeBuilder<Metal> builder)
        {
            builder.ToTable("Metal");
            builder.Property(o => o.Id)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => MetalId.Parse(dbValue));
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Id);
            builder.HasData(METALS);
            builder.OwnsOne(o => o.Thumbnail);
        }
    }
}
