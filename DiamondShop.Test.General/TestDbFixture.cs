using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.General
{
    public class TestDbFixture : IDisposable
    {
        public TestDbContext _context { get; set; }
        public TestDbFixture()
        {
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase($"Db_{new Guid().ToString()}")
                .ConfigureWarnings(p => p.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;
            _context = new TestDbContext(options);
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
