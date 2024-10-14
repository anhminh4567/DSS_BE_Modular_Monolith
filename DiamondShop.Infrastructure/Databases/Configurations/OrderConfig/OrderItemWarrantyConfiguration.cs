using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.OrderConfig
{
    internal class OrderItemWarrantyConfiguration : IEntityTypeConfiguration<OrderItemWarranty>
    {
        public void Configure(EntityTypeBuilder<OrderItemWarranty> builder)
        {
            builder.Property(o => o.Id)
                .HasConversion(
                 o => o.Value,
                dbValue => OrderItemWarrantyId.Parse(dbValue));
            builder.Property(o => o.OrderItemId)
               .HasConversion(
                o => o.Value,
               dbValue => OrderItemId.Parse(dbValue));
            builder.Property(o => o.WarrantyType).HasConversion<string>();
            builder.Property(o => o.Status).HasConversion<string>();
            builder.HasKey(o => new { o.Id });

        }
    }
}
