using DiamondShop.Domain.Models.News;
using DiamondShop.Domain.Models.News.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.NewsConfig
{
    internal class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.ToTable("News");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => NewsId.Parse(dbValue));
            builder.Property(o => o.PriorityLevel).HasConversion<string>();
            builder.HasKey(o => o.Id);
        }
    }
}
