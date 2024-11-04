using DiamondShop.Infrastructure.Databases;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DiamondShop.Test.Application.Integrations
{
    public class InitializeIntegration : WebApplicationFactory<Program>, IAsyncDisposable
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<DiamondShopDbContext>));
                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }
                var quartzDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(Quartz.IScheduler));
                services.Remove(quartzDescriptor);
                services.AddDbContext<DiamondShopDbContext>(opt =>
                {
                    // co the thay the bang docker, nhung chua biet cach, dang tim hieu, hien dang xai real db de test cho de
                    opt.UseNpgsql("Host=localhost;Port=5432;Database=diamondshop_testdb;Username=postgres;Password=12345;Include Error Detail=true");
                });
            });
        }
    }
}
