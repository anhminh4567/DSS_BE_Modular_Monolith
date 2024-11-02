using DiamondShop.Domain.Models.CustomizeRequests.Entities;
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
    }
}
