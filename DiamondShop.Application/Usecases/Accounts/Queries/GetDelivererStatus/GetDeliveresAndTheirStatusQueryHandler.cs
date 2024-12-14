using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Queries.GetDelivererStatus
{
    public record GetDeliveresAndTheirStatusQuery() : IRequest<List<DelivererStatusDto>>;
    internal class GetDeliveresAndTheirStatusQueryHandler : IRequestHandler<GetDeliveresAndTheirStatusQuery, List<DelivererStatusDto>>
    {
        private readonly IAccountServices _accountServices;
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountRoleRepository _accountRoleRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetDeliveresAndTheirStatusQueryHandler(IAccountServices accountServices, IAccountRepository accountRepository, IAuthenticationService authenticationService, IAccountRoleRepository accountRoleRepository, IOrderRepository orderRepository, IMapper mapper)
        {
            _accountServices = accountServices;
            _accountRepository = accountRepository;
            _authenticationService = authenticationService;
            _accountRoleRepository = accountRoleRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<List<DelivererStatusDto>> Handle(GetDeliveresAndTheirStatusQuery request, CancellationToken cancellationToken)
        {
            var response = new List<DelivererStatusDto>();
            var getALlRoles = await _accountRoleRepository.GetRoles();
            var deliveryRole = getALlRoles.Where(x => x.Id == AccountRole.Deliverer.Id).ToList();
            var getDeliverer  = await _accountRepository.GetByRoles(deliveryRole,cancellationToken);
            foreach(var deliverer in getDeliverer)
            {
                var orderCurrentlyHandle = await _orderRepository.GetDelivererCurrentlyHandledOrder(deliverer);
                if(orderCurrentlyHandle != null)
                {
                    response.Add(new DelivererStatusDto
                    {
                        Account = _mapper.Map<AccountDto>(deliverer),
                        OrderCurrentlyHandle = _mapper.Map<OrderDto>(orderCurrentlyHandle),
                        IsFree = false,
                        BusyMessage = "Đang giao cho đơn hàng với mã " + orderCurrentlyHandle.OrderCode
                    });
                }
                else
                {
                    var mappedDeliverer = new DelivererStatusDto
                    {
                        Account = _mapper.Map<AccountDto>(deliverer),
                        IsFree = true,
                    };
                    if (deliverer.Status == Domain.Models.AccountAggregate.Enums.AccountStatus.Banned)
                    {
                        mappedDeliverer.IsFree = false;
                        mappedDeliverer.BusyMessage = "tài khoản người giao hàng đã bị khóa";
                    }
                    response.Add(mappedDeliverer);
                }
            }
            return response;
        }
    }
}
