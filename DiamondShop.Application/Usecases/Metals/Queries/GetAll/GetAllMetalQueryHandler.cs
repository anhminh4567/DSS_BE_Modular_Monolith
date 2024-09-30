using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Metals.Queries.GetAll
{
    public record GetAllMetalQuery : IRequest<List<Metal>>;
    internal class GetAllMetalQueryHandler : IRequestHandler<GetAllMetalQuery,List<Metal>>
    {
        private readonly IMetalRepository _metalRepository;
        public GetAllMetalQueryHandler(IMetalRepository metalRepository)
        {
            _metalRepository = metalRepository;
        }
        public async Task<List<Metal>> Handle(GetAllMetalQuery request, CancellationToken cancellationToken)
        {
            var query = _metalRepository.GetQuery();
            return query.ToList();
        }
    }
}
