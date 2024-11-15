using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders.Events;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.DomainEventConsumers
{
    internal class OrderCompleteConsumer : INotificationHandler<OrderCompleteEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IOrderFileServices _orderFileServices;

        public OrderCompleteConsumer(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IOrderFileServices orderFileServices)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
            _optionsMonitor = optionsMonitor;
            _orderFileServices = orderFileServices;
        }

        public async Task Handle(OrderCompleteEvent notification, CancellationToken cancellationToken)
        {
            var getAccount = await _accountRepository.GetById(notification.AccountId);
            if(getAccount is null)
            {
                return;
            }
            var GetOrder = await _orderRepository.GetById(notification.OrderId);
            if(GetOrder is null)
            {
                return;
            }
            var getOrderPoint = AccountServices.GetUserPointFromOrderCompleteGlobal(getAccount,GetOrder);
            getAccount.AddTotalPoint(getOrderPoint);
            var getAllUserRoles = (await _accountRoleRepository.GetRoles()).Where(x => x.RoleType == Domain.Models.AccountRoleAggregate.AccountRoleType.Customer);
            var goldRank = getAllUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerGold.Id);
            var silverRank = getAllUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerSilver.Id);
            var bronzeRank = getAllUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerBronze.Id);
            var option = _optionsMonitor.CurrentValue.AccountRules;
            await AccountServices.CheckAndUpdateUserRankIfQualifiedGlobal(new List<Account>() { getAccount }, getAllUserRoles.ToList(), option);
            await _unitOfWork.SaveChangesAsync();
            //dont care about it, just run
            _orderFileServices.CreateOrderInvoice(GetOrder);
        }
    }
}
