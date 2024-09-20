using DiamondShop.Domain.Models.DiamondPrices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.DiamondPriceConfig
{
    internal class DiamondPriceConfiguration : IEntityTypeConfiguration<DiamondPrice>
    {
        public void Configure(EntityTypeBuilder<DiamondPrice> builder)
        {
            builder.ToTable("DiamondPrice");
            builder.HasOne(o => o.Shape).WithMany().HasForeignKey(o => o.ShapeId).IsRequired();
            builder.HasOne(o => o.Criteria).WithMany().HasForeignKey(o => o.CriteriaId).IsRequired();
            builder.HasKey(o => new { o.ShapeId, o.CriteriaId });
        }
    }
}
