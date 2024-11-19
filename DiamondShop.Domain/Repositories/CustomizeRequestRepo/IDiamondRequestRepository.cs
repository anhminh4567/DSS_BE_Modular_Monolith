using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.CustomizeRequestRepo
{
    public interface IDiamondRequestRepository : IBaseRepository<DiamondRequest>
    {
        public Task CreateRange(List<DiamondRequest> requests);
        Task<DiamondRequest?> GetById(DiamondRequestId diamondRequestId, CancellationToken cancellationToken = default);
    }
}
