using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.CustomizeRequestConfig
{
    internal class DiamondRequestConfiguration : IEntityTypeConfiguration<DiamondRequest>
    {
        public void Configure(EntityTypeBuilder<DiamondRequest> builder)
        {
            builder.Property(o => o.DiamondRequestId  )
                .HasConversion(
                    Id => Id.Value,
                    dbValue => DiamondRequestId.Parse(dbValue));
            builder.Property(o => o.CustomizeRequestId)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => CustomizeRequestId.Parse(dbValue));
            builder.Property(o => o.DiamondShapeId)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => DiamondShapeId.Parse(dbValue));
            builder.Property(o => o.DiamondId)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => DiamondId.Parse(dbValue));
            builder.Property(o => o.Cut).HasConversion<string>();
            builder.Property(o => o.Color).HasConversion<string>();
            builder.Property(o => o.Clarity).HasConversion<string>();
            builder.Property(o => o.Polish).HasConversion<string>();
            builder.Property(o => o.Symmetry).HasConversion<string>();  
            builder.Property(o => o.Girdle).HasConversion<string>();  
            builder.Property(o => o.Culet).HasConversion<string>();
            builder.HasKey(o => new { o.DiamondRequestId, o.CustomizeRequestId });
            builder.HasOne<DiamondShape>()
                .WithMany()
                .HasForeignKey(c => c.DiamondShapeId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<Diamond>()
                .WithMany()
                .HasForeignKey(c => c.DiamondId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
}
