using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Notifications.ValueObjects;
using DiamondShop.Domain.Models.Notifications;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders.ValueObjects;

namespace DiamondShop.Infrastructure.Databases.Configurations.NotificationConfig
{
    internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notification");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => NotificationId.Parse(dbValue));
            builder.Property(o => o.AccountId)
            .HasConversion(
                Id => Id.Value,
                dbValue => AccountId.Parse(dbValue));
            builder.Property(o => o.OrderId)
            .HasConversion(
                Id => Id.Value,
                dbValue => OrderId.Parse(dbValue));
            builder.HasOne(o => o.Account).WithOne().HasForeignKey<Notification>(o => o.AccountId).IsRequired(false);
            builder.HasOne(o => o.Order).WithOne().HasForeignKey<Notification>(o => o.OrderId).IsRequired(false);
            builder.HasKey(o => o.Id);

        }
    }
}
