using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Models.DeliveryFees.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
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
            builder.Property(o => o.Id)
            .HasConversion(
                 Id => Id.Value,
                 dbValue => DeliveryFeeId.Parse(dbValue));

            builder.HasKey(o => o.Id);
        }
    }
}
