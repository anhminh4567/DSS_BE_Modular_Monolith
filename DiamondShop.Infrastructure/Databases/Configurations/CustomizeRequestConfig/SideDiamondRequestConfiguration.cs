//using DiamondShop.Domain.Models.CustomizeRequests;
//using DiamondShop.Domain.Models.CustomizeRequests.Entities;
//using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
//using DiamondShop.Domain.Models.JewelryModels.Entities;
//using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DiamondShop.Infrastructure.Databases.Configurations.CustomizeRequestConfig
//{
//    internal class SideDiamondRequestConfiguration : IEntityTypeConfiguration<SideDiamondRequest>
//    {
//        public void Configure(EntityTypeBuilder<SideDiamondRequest> builder)
//        {
//            builder.Property(o => o.CustomizeRequestId)
//            .HasConversion(
//                Id => Id.Value,
//                dbValue => CustomizeRequestId.Parse(dbValue));
//            builder.Property(o => o.SideDiamondReqId)
//            .HasConversion(
//                Id => Id.Value,
//                dbValue => SideDiamondReqId.Parse(dbValue));
//            builder.HasOne<CustomizeRequest>().WithMany(o => o.SideDiamondRequests).HasForeignKey(o => o.CustomizeRequestId).OnDelete(DeleteBehavior.Cascade);
//            builder.HasOne<SideDiamondReq>().WithMany().HasForeignKey(o => o.SideDiamondReqId).OnDelete(DeleteBehavior.Cascade);

//            builder.HasKey(o => new { o.SideDiamondReqId, o.CustomizeRequestId });

//        }
//    }
//}
