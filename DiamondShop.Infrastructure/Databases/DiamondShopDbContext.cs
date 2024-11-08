using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.Locations;
using DiamondShop.Domain.Models.News;
using DiamondShop.Domain.Models.Notifications;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Infrastructure.Databases.Configurations.AccountConfig;
using DiamondShop.Infrastructure.Databases.Interceptors;
using DiamondShop.Infrastructure.Identity.Models;
using DiamondShop.Infrastructure.Outbox;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases
{
    public class DiamondShopDbContext : IdentityDbContext<
        CustomIdentityUser,
        CustomIdentityRole,
        string,
        CustomIdentityUserClaims,
        CustomIdentityUserRole,
        CustomIdentityUserLogins,
        CustomIdentityUserRoleClaim,
        CustomIdentityUserToken>
    {
        private readonly DomainEventsPublishserInterceptors _domainEventsInterceptor;

        public DiamondShopDbContext(DbContextOptions options, DomainEventsPublishserInterceptors interceptors) : base(options)
        {
            _domainEventsInterceptor = interceptors;
        }

        public DiamondShopDbContext()
        {
        }
        // Outbox Related //
        internal DbSet<OutboxMessages> OutboxMessages { get; set; }
        internal DbSet<ApplicationSettings> ApplicationSettings{ get; set; }
        //Roles Related//
        #region Dbset
        public DbSet<AccountRole> AccountRoles { get; set; }
        //public DbSet<AppProvince> AppProvinces { get; set; }
        public DbSet<AppCities> AppCities { get; set; }
        //public DbSet<AppDistrict> AppDistricts { get; set; }
        //public DbSet<AppWard> AppWards { get; set; }
        //Roles Related//

        //Application//
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<DiamondPrice> DiamondPrices { get; set; }
        public DbSet<DiamondCriteria> DiamondCriteria { get; set; }
        public DbSet<DiamondShape> DiamondShapes { get; set; }
        public DbSet<Diamond> Diamonds { get; set; }

        public DbSet<Jewelry> Jewelrys { get; set; }
        public DbSet<JewelryReview> JewelryReviews { get; set; }
        public DbSet<MainDiamondReq> MainDiamonds { get; set; }
        public DbSet<MainDiamondShape> MainDiamondShapes { get; set; }
        public DbSet<SideDiamondOpt> SideDiamondOpts { get; set; }
        public DbSet<JewelryModelCategory> JewelryModelCategories { get; set; }
        public DbSet<JewelryModel> JewelryModels { get; set; }
        public DbSet<Metal> Metals { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<SizeMetal> SizeMetals { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<PromoReq> PromoReqs { get; set; }
        public DbSet<PromoReqShape> PromoReqShapes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        #endregion
        //Application//

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlServer("server=(local);Uid=sa;Pwd=12345;Database=BeatVisionRemake;TrustServerCertificate=true");
            //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=DiamondShopTest;Username=postgres;Password=12345;Include Error Detail=true");
            //configure interceptors
            optionsBuilder.AddInterceptors(_domainEventsInterceptor);
            //configure logging
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*System.Diagnostics.Debugger.Launch();*/
            IdentityConfiguration.ApplyIdentityConfiguration(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiamondShopDbContext).Assembly);
        }

    }
}
