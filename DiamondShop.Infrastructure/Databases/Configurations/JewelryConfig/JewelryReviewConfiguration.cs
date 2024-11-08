using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.JewelryConfig
{
    internal class JewelryReviewConfiguration : IEntityTypeConfiguration<JewelryReview>
    {
        public void Configure(EntityTypeBuilder<JewelryReview> builder)
        {
            builder.ToTable("JewelryReview");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => JewelryId.Parse(dbValue));
            builder.Property(o => o.AccountId)
            .HasConversion(
                Id => Id.Value,
                dbValue => AccountId.Parse(dbValue));
            builder.HasOne(o => o.Account).WithMany().HasForeignKey(o => o.AccountId);
            builder.OwnsMany(o => o.Images, childNavigation =>
            {
                childNavigation.ToJson();
            });
            builder.HasKey(o => o.Id);
        }
    }
}
