using DiamondShop.Domain.Models.Orders.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.ValueObjects;

namespace DiamondShop.Infrastructure.Databases.Configurations.OrderConfig
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => OrderId.Parse(dbValue));
            builder.HasOne(o => o.Account).WithOne().HasForeignKey<Order>(o => o.AccountId).IsRequired();
            builder.Property(o => o.Status).HasConversion<string>();
            builder.Property(o => o.PaymentStatus).HasConversion<string>();
            builder.HasOne(o => o.Transaction).WithMany(p => p.Orders).HasForeignKey(o => o.TransactionId).IsRequired(false);
            builder.HasMany(o => o.Items).WithOne().HasForeignKey(p => p.OrderId).IsRequired();
            builder.HasMany(o => o.Logs).WithOne().HasForeignKey(p => p.OrderId).IsRequired();
            builder.HasKey(o => o.Id);
        }
    }
}
