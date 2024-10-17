using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Warranties;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Domain.Models.Warranties.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.WarrantyConfig
{
    internal class WarrantyConfiguration : IEntityTypeConfiguration<Warranty>
    {
        private static List<Warranty> WARRANTIES = new()
        {
            Warranty.Create(WarrantyType.Jewelry, "Default_Jewelry_Warranty",nameof(WarrantyRules.THREE_MONTHS),WarrantyRules.THREE_MONTHS,0,WarrantyId.Parse(1.ToString())),
            Warranty.Create(WarrantyType.Diamond, "Default_Diamond_Warranty",nameof(WarrantyRules.THREE_MONTHS),WarrantyRules.THREE_MONTHS,0,WarrantyId.Parse(2.ToString()))
        };
        new public void Configure(EntityTypeBuilder<Warranty> builder)
        {
            builder.Property(o => o.Id)
                .HasConversion(
                 o => o.Value,
                 dbValue => WarrantyId.Parse(dbValue));
            builder.Property(o => o.Type).HasConversion<string>();
            builder.HasKey(o => o.Id);
            builder.HasData(WARRANTIES);
        }   
    }
}
