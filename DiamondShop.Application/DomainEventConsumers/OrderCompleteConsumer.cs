using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Orders.Events;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.Implementations;
using MediatR;
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

        public OrderCompleteConsumer(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IAccountRepository accountRepository)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
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
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
