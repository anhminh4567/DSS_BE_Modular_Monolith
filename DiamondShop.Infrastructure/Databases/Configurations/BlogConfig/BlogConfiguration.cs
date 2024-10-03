using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Models.Blogs.Entities;
using DiamondShop.Domain.Models.Blogs.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.BlogConfig
{
    internal class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("Blog");
            builder.Property(o => o.Id)
               .HasConversion(
                   Id => Id.Value,
                   dbValue => BlogId.Parse(dbValue));
            builder.Property(o => o.AccountId)
               .HasConversion(
                   Id => Id.Value,
                   dbValue => AccountId.Parse(dbValue));
            builder.Property(o => o.Tags).IsRequired(false).HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                dbValue => JsonConvert.DeserializeObject<List<BlogTag>>(dbValue, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                );
            builder.Property(o => o.Medias).IsRequired(false).HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                dbValue => JsonConvert.DeserializeObject<List<BlogMedia>>(dbValue, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                );
            builder.OwnsOne(o => o.Thumbnail, childBuilder =>
            {
                childBuilder.ToJson();
            });
            builder.HasKey(o => o.Id);
        }
    }
}
