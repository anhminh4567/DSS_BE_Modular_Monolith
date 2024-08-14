using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.CustomerAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthenticationService _authenticationService;


        public CustomerRepository(DiamondShopDbContext dbContext, IAuthorizationService authorizationService, IAuthenticationService authenticationService) : base(dbContext)
        {
            _authorizationService = authorizationService;
            _authenticationService = authenticationService;
        }

        public override async Task<Customer?> GetById(CancellationToken token = default, params object[] ids)
        {
            var find = await _set.FindAsync(ids);
            if(find == null)
            {
                return null;
            }
            var getResult= await _authenticationService.GetUserIdentity(find.IdentityId,token);
            if(getResult.IsSuccess is false ) 
            {
                throw new NullReferenceException("Cannot found user identity");
            }
            find.SetIdentity(getResult.Value);
            return find;
        }

        public async Task AddRole(IUserIdentity identity, DiamondShopCustomerRole diamondShopCustomerRole, CancellationToken cancellationToken = default)
        {
            await _authorizationService.AddToRole(identity, diamondShopCustomerRole.Value, cancellationToken);
        }

        public async Task RemoveRole(IUserIdentity identity, DiamondShopCustomerRole diamondShopCustomerRole, CancellationToken cancellationToken = default)
        {
            await _authorizationService.RemoveFromRole(identity,diamondShopCustomerRole.Value,cancellationToken);
        }
    }
}
