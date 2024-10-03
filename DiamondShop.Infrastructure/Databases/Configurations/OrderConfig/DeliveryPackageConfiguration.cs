using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.OrderConfig
{
    internal class DeliveryPackageConfiguration : IEntityTypeConfiguration<DeliveryPackage>
    {
        public void Configure(EntityTypeBuilder<DeliveryPackage> builder)
        {
            builder.Property(p => p.Id)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => DeliveryPackageId.Parse(dbValue));
            builder.Property(p => p.DelivererId)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => AccountId.Parse(dbValue));
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Deliverer).WithMany()
                .HasForeignKey(p => p.DelivererId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            builder.HasKey(p => p.Id);
        }
    }
}
