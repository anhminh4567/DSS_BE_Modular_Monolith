using DiamondShop.Domain.Models.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.Cities
{
    internal class AppCitiesConfiguration : IEntityTypeConfiguration<AppCities>
    {
        public void Configure(EntityTypeBuilder<AppCities> builder)
        {
            builder.HasIndex(o => o.Slug);
            builder.HasIndex(o => o.Name);
            builder.Property(o => o.Id)
                .ValueGeneratedNever();

            builder.HasKey(o => o.Id);
        }
    }
    //internal class AppDistrictConfiguration : IEntityTypeConfiguration<AppDistrict>
    //{
    //    public void Configure(EntityTypeBuilder<AppDistrict> builder)
    //    {
    //        builder.Property(o => o.Id)
    //            .ValueGeneratedNever();

    //        builder.HasIndex(o => o.Name);
    //        builder.HasKey(o => o.Id);
    //        builder.HasIndex(o => o.ProvinceId);
    //    }
    //}
    //internal class AppWardConfiguration : IEntityTypeConfiguration<AppWard>
    //{
    //    public void Configure(EntityTypeBuilder<AppWard> builder)
    //    {
    //        builder.Property(o => o.Id)
    //            .ValueGeneratedNever();

    //        builder.HasIndex(o => o.Name);
    //        builder.HasKey(o => o.Id);
    //        builder.HasIndex(o => o.DistrictId);
    //    }
    //}
}
