using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Sizes.Queries.GetAll
{
    public record GetAllSizeQuery() : IRequest<List<Size>>;
    internal class GetAllSizeQueryHandler : IRequestHandler<GetAllSizeQuery, List<Size>>
    {
        private readonly ISizeRepository _sizeRepository;

        public GetAllSizeQueryHandler(ISizeRepository sizeRepository)
        {
            _sizeRepository = sizeRepository;
        }
        public async Task<List<Size>> Handle(GetAllSizeQuery request, CancellationToken cancellationToken)
        {
            var query = _sizeRepository.GetQuery();
            var list =query
                .OrderBy(x => x.Unit == Size.Milimeter ? 0 : x.Unit == Size.Centimeter ? 1 : 2)
                .ThenBy(x => x.Value)
                .ToList();
            return list;
        }
    }
}
