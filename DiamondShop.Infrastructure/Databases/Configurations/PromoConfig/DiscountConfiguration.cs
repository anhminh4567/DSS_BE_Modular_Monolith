using DiamondShop.Domain.Models.Promotions.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Promotions.ValueObjects;

namespace DiamondShop.Infrastructure.Databases.Configurations.PromoConfig
{
    internal class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable("Discount");
            builder.Property(o => o.Id)
             .HasConversion(
                 o => o.Value,
                 dbValue => DiscountId.Parse(dbValue));
            builder.OwnsOne(o => o.Thumbnail, childBuilder =>
            {
                childBuilder.ToJson();
            });
            builder.HasKey(o => o.Id);
            builder.HasQueryFilter(x => x.Status != Domain.Models.Promotions.Enum.Status.Soft_deleted);
        }
    }
}
