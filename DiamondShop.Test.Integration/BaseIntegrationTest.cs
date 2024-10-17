using DiamondShop.Infrastructure.Databases;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DiamondShop.Test.Integration
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationWAF>, IDisposable
    {
        private readonly IServiceScope _scope;
        protected readonly ISender _sender;
        protected readonly DiamondShopDbContext _context;

        public BaseIntegrationTest(IntegrationWAF factory)
        {
            _scope = factory.Services.CreateScope();
            _sender = _scope.ServiceProvider.GetRequiredService<ISender>();
            _context = _scope.ServiceProvider.GetRequiredService<DiamondShopDbContext>();
            _context.Database.EnsureCreated();
        }
        protected static List<T> GetTestData<T>(string folder, string file) where T : class
        {
            var jsonData = File.ReadAllText(Path.Combine("Data", folder, $"{file}.json"));
            var data = JsonConvert.DeserializeObject(jsonData, typeof(List<T>)) as List<T>;
            return data;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
