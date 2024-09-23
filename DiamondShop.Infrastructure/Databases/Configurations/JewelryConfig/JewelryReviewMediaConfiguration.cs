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
/*    internal class JewelryReviewMediaConfiguration : IEntityTypeConfiguration<JewelryReviewMedia>
    {
        public void Configure(EntityTypeBuilder<JewelryReviewMedia> builder)
        {
            builder.ToTable("JewelryReviewMedia");
            builder.Property(o => o.Id)
                .HasConversion(
                o => o.Value,
                dbValue => JewelryReviewMediaId.Parse(dbValue));
            builder.HasKey(o => o.Id);
            builder.HasIndex(o => o.Id);
        }
    }*/
}
