using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Repositories.JewelryRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetAll
{
    public record GetAllJewelryQuery : IRequest<List<Jewelry>>;
    internal class GetAllJewelryQueryHandler : IRequestHandler<GetAllJewelryQuery, List<Jewelry>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        public GetAllJewelryQueryHandler(
            IJewelryRepository jewelryRepository
            )
        {
            _jewelryRepository = jewelryRepository;
        }

        public async Task<List<Jewelry>> Handle(GetAllJewelryQuery request, CancellationToken cancellationToken)
        {
            var query = _jewelryRepository.GetQuery();
            return query.ToList();
        }
    }
}
