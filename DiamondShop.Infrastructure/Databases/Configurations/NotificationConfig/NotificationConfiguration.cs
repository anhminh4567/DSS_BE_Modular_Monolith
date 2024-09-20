using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Notifications.ValueObjects;
using DiamondShop.Domain.Models.Notifications;

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
            builder.HasOne(o => o.Account).WithOne().HasForeignKey<Notification>(o => o.AccountId).IsRequired();
            builder.HasOne(o => o.Order).WithOne().HasForeignKey<Notification>(o => o.OrderId).IsRequired();
            builder.HasKey(o => o.Id);

        }
    }
}
