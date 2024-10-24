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
    internal class OrderLogConfiguration : IEntityTypeConfiguration<OrderLog>
    {
        public void Configure(EntityTypeBuilder<OrderLog> builder)
        {
            builder.ToTable("OrderLog");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => OrderLogId.Parse(dbValue));
            builder.Property(o => o.OrderId)
            .HasConversion(
                Id => Id.Value,
                dbValue => OrderId.Parse(dbValue));
            builder.Property(o => o.PreviousLogId)
            .HasConversion(
                Id => Id.Value,
                dbValue => OrderLogId.Parse(dbValue));
            builder.OwnsMany(o => o.LogImages, childBuilder =>
            {
                childBuilder.WithOwner();
                childBuilder.ToJson();
            });
            builder.HasOne(o => o.PreviousLog).WithOne().HasForeignKey<OrderLog>(o => o.PreviousLogId).IsRequired(false);
            builder.Property(o => o.Status).HasConversion<string>();
            builder.HasKey(o => o.Id);
        }
    }
}
