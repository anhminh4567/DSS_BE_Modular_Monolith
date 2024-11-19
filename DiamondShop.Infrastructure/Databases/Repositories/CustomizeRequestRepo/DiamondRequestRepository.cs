using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.CustomizeRequestRepo
{
    internal class DiamondRequestRepository : BaseRepository<DiamondRequest>, IDiamondRequestRepository
    {
        public DiamondRequestRepository(DiamondShopDbContext context) : base(context) { }
        public async Task CreateRange(List<DiamondRequest> requests)
        {
            await _set.AddRangeAsync(requests);
        }

        public Task<DiamondRequest?> GetById(DiamondRequestId diamondRequestId, CancellationToken cancellationToken = default)
        {
            return _set.Include(p => p.Diamond).FirstOrDefaultAsync(x => x.DiamondRequestId == diamondRequestId);
        }
    }
}
