using DiamondShop.Infrastructure.Databases;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.Application.Integrations
{
    /// <summary>
    /// test using real db
    /// </summary>
    public class BaseTest : IClassFixture<InitializeIntegration>
    {
        protected readonly IServiceScope _scope;
        protected readonly ISender _sender;
        protected readonly DiamondShopDbContext _context;

        public BaseTest(InitializeIntegration factory)
        {
            _scope = factory.Services.CreateScope();
            _sender = _scope.ServiceProvider.GetRequiredService<ISender>();
            _context = _scope.ServiceProvider.GetRequiredService<DiamondShopDbContext>();
        }
    }
}
