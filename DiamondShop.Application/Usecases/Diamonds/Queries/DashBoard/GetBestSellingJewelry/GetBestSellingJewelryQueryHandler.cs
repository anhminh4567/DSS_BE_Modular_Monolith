using DiamondShop.Application.Dtos.Responses.Dashboard;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.DashBoard.GetBestSellingJewelry
{
    public record GetBestSellingJewelryQuery() : IRequest<Result<List<JewelryTopSellingDto>>>;
    internal class GetBestSellingJewelryQueryHandler : IRequestHandler<GetBestSellingJewelryQuery, Result<List<JewelryTopSellingDto>>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        public GetBestSellingJewelryQueryHandler(IJewelryRepository jewelryRepository, IOrderItemRepository orderItemRepository)
        {
            _jewelryRepository = jewelryRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<Result<List<JewelryTopSellingDto>>> Handle(GetBestSellingJewelryQuery request, CancellationToken token)
        {
            var orderItems = _orderItemRepository.GetSoldJewelry().ToList();
            var jewelryGroup = orderItems.GroupBy(p => new { 
                ModelName = p.Jewelry.Model.Name, 
                MetalName = p.Jewelry.Metal.Name 
            }).Select(p => new { p.Key.ModelName, p.Key.MetalName, Revenue = p.Sum(p => p.PurchasedPrice)} );
            return jewelryGroup.Select(p => new JewelryTopSellingDto(p.ModelName,p.MetalName,p.Revenue)).OrderByDescending(p => p.Revenue).ToList();
        }
    }
}
