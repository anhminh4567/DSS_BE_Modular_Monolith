using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Promotions.Entities;

namespace DiamondShop.Infrastructure.Databases.Configurations.PromoConfig
{
    internal class PromoReqShapeConfiguration : IEntityTypeConfiguration<PromoReqShape>
    {
        public void Configure(EntityTypeBuilder<PromoReqShape> builder)
        {
            builder.ToTable("PromoReqShape");
            builder.HasOne(o => o.PromoReq).WithMany().HasForeignKey(o => o.PromoReqId).IsRequired();
            builder.HasOne(o => o.DiamondShape).WithMany().HasForeignKey(o => o.DiamondShapeId).IsRequired();
            builder.HasKey(o => new { o.PromoReqId, o.DiamondShapeId});
        }
    }
}
