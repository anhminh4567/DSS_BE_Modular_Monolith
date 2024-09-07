using DiamondShop.Domain.Models.RoleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Outbox
{
    internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessages>
    {
        public void Configure(EntityTypeBuilder<OutboxMessages> builder)
        {
            builder.ToTable("outbox_message");
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Id);
        }
    }
}
