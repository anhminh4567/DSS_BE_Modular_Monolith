using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Dtos.Responses.Transactions;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using DiamondShop.Domain.Repositories.TransactionRepo;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Transactions.Queries.GetAllMethodsAtShop
{
    public record GetAllPaymentMethodAtShopQuery() : IRequest<List<PaymentMethodDto>>;
    internal class GetAllPaymentMethodAtShopQueryHandler : IRequestHandler<GetAllPaymentMethodAtShopQuery, List<PaymentMethodDto>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public async Task<List<PaymentMethodDto>> Handle(GetAllPaymentMethodAtShopQuery request, CancellationToken cancellationToken)
        {
            var paymentMethodRules = _optionsMonitor.CurrentValue.OrderPaymentRules;
            var httpContext = _httpContextAccessor.HttpContext;
            var result = await _paymentMethodRepository.GetAll();
            //get onlyl active method
            result = result.Where(x => x.Status == true).ToList();
            if (httpContext != null)
            {
                var getRole = httpContext.User.GetUserRoles();
                if (getRole != null)
                {
                    var shopRoles = AccountRole.ShopRoles.Select(x => x.Id.Value).ToList();
                    foreach (var role in getRole)
                    {
                        if (shopRoles.Contains(role))
                        {

                            return _mapper.Map<List<PaymentMethodDto>>(result);
                        }
                    }
                }
            }
            foreach (var lockedMethodId in paymentMethodRules.LockedPaymentMethodOnCustomer)
            {
                var anyFoujnd = result.Where(x => x.Id == PaymentMethodId.Parse(lockedMethodId)).FirstOrDefault();
                if (anyFoujnd != null)
                    result.Remove(anyFoujnd);
            }//result.Where(x => x.Id != PaymentMethod.CASH.Id.ToList())
            return _mapper.Map<List<PaymentMethodDto>>(result);
        }
    }
}
