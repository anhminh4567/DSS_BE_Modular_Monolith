using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.ValueObjects;

namespace DiamondShop.Infrastructure.Databases.Configurations.TransactionConfig
{
    internal class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public static List<PaymentMethod> Seed = new List<PaymentMethod>
        {
            PaymentMethod.BANK_TRANSFER,
            PaymentMethod.ZALOPAY
            //PaymentMethod.Create("BANK_TRANSFER","1"),
            //PaymentMethod.Create("ZALOPAY","2")
        };
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("PaymentMethod");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => PaymentMethodId.Parse(dbValue));
            builder.HasKey(o => o.Id);
            builder.HasData(Seed);
        }

    }
}