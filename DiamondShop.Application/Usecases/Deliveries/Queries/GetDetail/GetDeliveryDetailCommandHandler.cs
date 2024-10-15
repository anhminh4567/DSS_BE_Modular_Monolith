using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Deliveries.Queries.GetDetail
{
    public record GetDeliveryDetailCommand(string deliveryId) : IRequest<Result<DeliveryPackage>>;
    internal class GetDeliveryDetailCommandHandler : IRequestHandler<GetDeliveryDetailCommand, Result<DeliveryPackage>>
    {
        public readonly IDeliveryPackageRepository _deliveryPackageRepository;
        public GetDeliveryDetailCommandHandler(IDeliveryPackageRepository deliveryPackageRepository)
        {
            _deliveryPackageRepository = deliveryPackageRepository;
        }
        public async Task<Result<DeliveryPackage>> Handle(GetDeliveryDetailCommand request, CancellationToken cancellationToken)
        {
            var query = _deliveryPackageRepository.GetQuery();
            query = _deliveryPackageRepository.QueryInclude(query, p => p.Orders);
            query = _deliveryPackageRepository.QueryFilter(query, p => p.Id == DeliveryPackageId.Parse(request.deliveryId));
            var delivery = query.FirstOrDefault();
            if (delivery == null)
                return Result.Fail("This delivery doesn't exist");
            return delivery;
        }
    }
}
