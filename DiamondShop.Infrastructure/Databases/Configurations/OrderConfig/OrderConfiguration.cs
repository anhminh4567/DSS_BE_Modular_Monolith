using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            builder.Property(o => o.AccountId).HasConversion(
                o => o.Value,
                dbValue => AccountId.Parse(dbValue));
            builder.Property(p => p.CustomizeRequestId)
            .HasConversion(
                Id => Id.Value,
                dbValue => CustomizeRequestId.Parse(dbValue));

            builder.HasOne(o => o.Account).WithMany().HasForeignKey(o => o.AccountId).IsRequired();

            builder.Property(o => o.Status).HasConversion<string>();
            builder.Property(o => o.PaymentType).HasConversion<string>();
            builder.Property(o => o.PaymentStatus).HasConversion<string>();

            builder.HasMany(o => o.Transactions).WithOne(k => k.Order).HasForeignKey(o => o.OrderId).IsRequired(false);
            builder.HasMany(o => o.Items).WithOne().HasForeignKey(p => p.OrderId).IsRequired();
            builder.HasMany(o => o.Logs).WithOne().HasForeignKey(p => p.OrderId).IsRequired();
            builder.HasOne(p => p.Promotion).WithMany().HasForeignKey(p => p.PromotionId).IsRequired(false);
            builder.HasOne(o => o.Account).WithMany().HasForeignKey(o => o.AccountId).IsRequired().OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(o => o.Deliverer).WithMany().HasForeignKey(o => o.DelivererId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.PaymentMethod)
                .WithMany()
                .HasForeignKey(x => x.PaymentMethodId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Navigation(x => x.PaymentMethod).AutoInclude();
            builder.HasKey(o => o.Id);
        }
    }
}
