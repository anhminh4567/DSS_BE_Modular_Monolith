using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.DiamondPriceConfig
{
    internal class DiamondCriteriaConfiguration : IEntityTypeConfiguration<DiamondCriteria>
    {
        public void Configure(EntityTypeBuilder<DiamondCriteria> builder)
        {
            builder.ToTable("DiamondCriteria");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => DiamondCriteriaId.Parse(dbValue));
            builder.Property(o => o.Cut).HasConversion<string>();
            builder.Property(o => o.Clarity).HasConversion<string>();
            builder.Property(o => o.Color).HasConversion<string>();
            builder.HasKey(o => o.Id);
        }
    }
}
