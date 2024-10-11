using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetDetail
{
    public record GetJewelryDetailQuery(string jewelryId) : IRequest<Result<Jewelry>>;
    internal class GetJewelryDetailQueryHandler : IRequestHandler<GetJewelryDetailQuery, Result<Jewelry>>
    {
        private readonly IJewelryRepository _jewelryRepository;

        public GetJewelryDetailQueryHandler(IJewelryRepository jewelryRepository)
        {
            _jewelryRepository = jewelryRepository;
        }

        public async Task<Result<Jewelry>> Handle(GetJewelryDetailQuery request, CancellationToken token)
        {
            var detail = _jewelryRepository.GetQuery().FirstOrDefault(p => p.Id == JewelryId.Parse(request.jewelryId));
            return detail;
        }
    }
}
