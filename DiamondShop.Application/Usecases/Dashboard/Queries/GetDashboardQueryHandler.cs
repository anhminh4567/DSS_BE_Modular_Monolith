using DiamondShop.Application.Dtos.Responses.Dashboard;
using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Commons;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Dashboard.Queries
{
    public record GetDashboardQuery : IRequest<Result<DashboardDto>>;
    internal class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, Result<DashboardDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public GetDashboardQueryHandler(IOrderRepository orderRepository, ICustomizeRequestRepository customizeRequestRepository, IJewelryRepository jewelryRepository, IDiamondRepository diamondRepository, IMemoryCache memoryCache, ITransactionRepository transactionRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _customizeRequestRepository = customizeRequestRepository;
            _jewelryRepository = jewelryRepository;
            _diamondRepository = diamondRepository;
            _memoryCache = memoryCache;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<Result<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            string key = $"Dashboard_Revenue";
            _memoryCache.TryGetValue(key, out DashboardDto dashboard);
            if (dashboard == null)
            {
                var orderQuery = _orderRepository.GetQuery();
                orderQuery = _orderRepository.QueryInclude(orderQuery, p => p.Transactions);
                var orders = orderQuery.ToList();
                var mappedOrders = _mapper.Map<List<OrderDashboardDto>>(orders);
                dashboard = new DashboardDto(mappedOrders);
                _memoryCache.Set(key, dashboard, TimeSpan.FromHours(5));
                return dashboard;
            }
            else
            {
                return dashboard;
            }
        }
    }
}
