using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Deliveries.Queries.GetUser
{
    public record GetUserDeliveryCommand(string staffId) : IRequest<Result<List<DeliveryPackage>>>;
    internal class GetUserDeliveryCommandHandler : IRequestHandler<GetUserDeliveryCommand, Result<List<DeliveryPackage>>>
    {
        public readonly IDeliveryPackageRepository _deliveryPackageRepository;
        public GetUserDeliveryCommandHandler(IDeliveryPackageRepository deliveryPackageRepository)
        {
            _deliveryPackageRepository = deliveryPackageRepository;
        }
        public async Task<Result<List<DeliveryPackage>>> Handle(GetUserDeliveryCommand request, CancellationToken cancellationToken)
        {
            var query = _deliveryPackageRepository.GetQuery();
            query = _deliveryPackageRepository.QueryFilter(query, p => p.DelivererId == AccountId.Parse(request.staffId));
            query = _deliveryPackageRepository.QueryOrderBy(query, p => p.OrderBy(k => k.Status));
            return query.ToList();
        }
    }
}
