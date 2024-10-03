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
            Metal.Create("Platinum", 778_370),
            Metal.Create("14K Yellow Gold", 1_217_096),
            Metal.Create("14K White Gold", 1_217_096),
            Metal.Create("14K Pink Gold", 1_217_096),
            Metal.Create("16K Yellow Gold", 1_391_318),
            Metal.Create("16K White Gold", 1_391_318),
            Metal.Create("16K Pink Gold", 1_391_318),
            Metal.Create("18K Yellow Gold", 1_565_233),
            Metal.Create("18K White Gold", 1_565_233),
            Metal.Create("18K Pink Gold", 1_565_233),
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
