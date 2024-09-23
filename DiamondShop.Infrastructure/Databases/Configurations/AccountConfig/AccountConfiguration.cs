using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.AccountConfig
{
    internal class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Account");
            builder.Property(o => o.Id)
               .HasConversion(
                   Id => Id.Value,
                   dbValue => AccountId.Parse(dbValue));
            builder.OwnsOne(o => o.FullName,
                fullname =>
                {
                    fullname.WithOwner();
                    fullname.Property(n => n.FirstName).HasColumnName("FirstName");  // Maps to FirstName column
                    fullname.Property(n => n.LastName).HasColumnName("LastName");
                    fullname.Ignore(n => n.Value);
                });
            builder.HasOne<CustomIdentityUser>()
                .WithOne()
                .HasForeignKey<Account>(c => c.IdentityId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Roles)
                .WithMany(a => a.Accounts);

            builder.HasMany(a => a.Addresses)
                .WithOne()
                .HasForeignKey(aa => aa.AccountId);

            builder.HasKey(o => o.Id);
            builder.HasIndex(o => o.Id);
            builder.HasIndex(o => o.IdentityId);
        }
    }
}
