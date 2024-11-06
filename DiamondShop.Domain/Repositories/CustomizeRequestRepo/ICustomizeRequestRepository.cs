using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.CustomizeRequestRepo
{
    public interface ICustomizeRequestRepository : IBaseRepository<CustomizeRequest>
    {
        public Task<CustomizeRequest> GetDetail(CustomizeRequestId requestId);
    }
}
