using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Models.DeliveryFees.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.DeliveryFeeConfig
{
    internal class DeliveryFeeConfiguration : IEntityTypeConfiguration<DeliveryFee>
    {
        public void Configure(EntityTypeBuilder<DeliveryFee> builder)
        {
            builder.ToTable("DeliveryFee");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => new DeliveryFeeId(dbValue));
            builder.Property(o => o.Method).HasConversion<string>();
            builder.HasKey(o => o.Id);
        }
    }
}
