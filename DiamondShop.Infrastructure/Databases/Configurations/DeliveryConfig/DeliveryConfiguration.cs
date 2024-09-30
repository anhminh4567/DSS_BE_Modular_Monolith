using DiamondShop.Domain.Models.Deliveries;
using DiamondShop.Domain.Models.Deliveries.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.DeliveryConfig
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.ToTable("Delivery");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => new DeliveryId(dbValue));
            builder.HasOne(o => o.Account).WithMany().HasForeignKey(o => o.AccountId);
            builder.HasMany(o => o.Orders).WithOne(p => p.Delivery).HasForeignKey(p => p.DeliveryId);
            builder.Property(o => o.Status).HasConversion<string>();
            builder.HasKey(o => o.Id);
        }
    }
}
