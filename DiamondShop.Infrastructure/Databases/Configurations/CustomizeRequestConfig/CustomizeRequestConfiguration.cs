﻿using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.CustomizeRequestConfig
{
    internal class CustomizeRequestConfiguration : IEntityTypeConfiguration<CustomizeRequest>
    {
        public void Configure(EntityTypeBuilder<CustomizeRequest> builder)
        {
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => CustomizeRequestId.Parse(dbValue));
            builder.Property(o => o.AccountId)
                .HasConversion(
                    o => o.Value,
                    dbValue => AccountId.Parse(dbValue));
            builder.Property(o => o.JewelryId)
            .HasConversion(
                Id => Id.Value,
                dbValue => JewelryId.Parse(dbValue));
            builder.Property(o => o.JewelryModelId)
                .HasConversion(
                    o => o.Value,
                    dbValue => JewelryModelId.Parse(dbValue));
            builder.Property(o => o.SizeId)
                .HasConversion(
                    o => o.Value,
                    dbValue => SizeId.Parse(dbValue));
            builder.Property(o => o.MetalId)
                .HasConversion(
                    o => o.Value,
                    dbValue => MetalId.Parse(dbValue));
            builder.Property(o => o.Status).HasConversion<string>();
            builder.HasKey(o => o.Id);
            builder.HasOne(o => o.Account)
                .WithMany()
                .HasForeignKey(o => o.AccountId);
            builder.HasOne(o => o.JewelryModel)
                .WithMany()
                .HasForeignKey(o => o.JewelryModelId);
            builder.HasOne(o => o.Size)
                .WithMany()
                .HasForeignKey(o => o.SizeId);
            builder.HasOne(o => o.Metal)
                .WithMany()
                .HasForeignKey(o => o.MetalId);
            builder.HasMany(o => o.DiamondRequests)
                .WithOne()
                .HasForeignKey(req => req.CustomizeRequestId);
            builder.HasOne(o => o.SideDiamond)
                .WithOne()
                .HasForeignKey<CustomizeRequest>(o => o.SideDiamondId)
                .IsRequired(false);
            builder.HasKey(o => o.Id);
        }
    }
}
