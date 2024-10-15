using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Deliveries.Queries.GetAll
{
    public record GetAllDeliveryCommand : IRequest<Result<List<DeliveryPackage>>>;
    internal class GetAllDeliveryCommandHandler : IRequestHandler<GetAllDeliveryCommand, Result<List<DeliveryPackage>>>
    {
        public readonly IDeliveryPackageRepository _deliveryPackageRepository;

        public GetAllDeliveryCommandHandler(IDeliveryPackageRepository deliveryPackageRepository)
        {
            _deliveryPackageRepository = deliveryPackageRepository;
        }

        public async Task<Result<List<DeliveryPackage>>> Handle(GetAllDeliveryCommand request, CancellationToken cancellationToken)
        {
            var query = _deliveryPackageRepository.GetQuery();
            query = _deliveryPackageRepository.QueryOrderBy(query, p => p.OrderBy(k => k.Status));
            return query.ToList();
        }
    }
}
