using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetAll
{
    public record GetAllJewelryModelQuery : IRequest<List<JewelryModel>>;
    internal class GetAllJewelryModelQueryHandler : IRequestHandler<GetAllJewelryModelQuery, List<JewelryModel>>
    {
        private readonly IJewelryModelRepository _jewelryRepository;
        public GetAllJewelryModelQueryHandler(IJewelryModelRepository jewelryModelRepository)
        {
            _jewelryRepository = jewelryModelRepository;
        }
        public async Task<List<JewelryModel>> Handle(GetAllJewelryModelQuery request, CancellationToken token)
        {
            var query = _jewelryRepository.GetQuery();
            return query.ToList();
        }
    }
}
