using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Locations;
using DiamondShop.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.AccountConfig
{
    internal class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");
            builder.Property(o => o.Id)
               .HasConversion(
                   Id => Id.Value,
                   dbValue => AddressId.Parse(dbValue));
            builder.Property(o => o.AccountId)
               .HasConversion(
                   Id => Id.Value,
                   dbValue => AccountId.Parse(dbValue));
            builder.HasKey(o => o.Id);
            builder.HasOne<AppCities>()
                .WithMany()
                .HasForeignKey(x => x.ProvinceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(aa => new { aa.Id, aa.AccountId });
        }
    }
}
