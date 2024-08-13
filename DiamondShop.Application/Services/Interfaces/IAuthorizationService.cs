using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface IAuthorizationService
    {
        Task AddToRole(IUserIdentity identity ,Role role, CancellationToken cancellationToken = default);
        Task RemoveFromRole(IUserIdentity identity, Role role, CancellationToken cancellationToken = default);
        Task Ban(IUserIdentity identity,TimeSpan time, CancellationToken cancellationToken = default);
    }
}
