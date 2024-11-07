using DiamondShop.Domain.Models.Warranties;
using DiamondShop.Domain.Models.Warranties.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Warranties.Queries.GetDetail
{
    public record GetDetailWarrantyQuery(string WarrantyId) : IRequest<Result<Warranty>>;
    internal class GetDetailWarrantyQueryHandler : IRequestHandler<GetDetailWarrantyQuery, Result<Warranty>>
    {
        private readonly IWarrantyRepository _warrantyRepository;

        public GetDetailWarrantyQueryHandler(IWarrantyRepository warrantyRepository)
        {
            _warrantyRepository = warrantyRepository;
        }

        public async Task<Result<Warranty>> Handle(GetDetailWarrantyQuery request, CancellationToken token)
        {
            request.Deconstruct(out string? warrantyId);
            var warranty = await _warrantyRepository.GetById(WarrantyId.Parse(warrantyId));
            if (warranty == null)
                return Result.Fail("This warranty doesn't exist");
            return warranty;
        }
    }
}
