using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondShapes.Queries.GetAll
{
    public record GetAllDiamondShapeQuery : IRequest<List<DiamondShape>>;
    internal class GetAllDiamondShapeQueryHandler : IRequestHandler<GetAllDiamondShapeQuery,List<DiamondShape>>
    {
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public GetAllDiamondShapeQueryHandler(IDiamondShapeRepository diamondShapeRepository)
        {
            _diamondShapeRepository = diamondShapeRepository;
        }

        public async Task<List<DiamondShape>> Handle(GetAllDiamondShapeQuery request, CancellationToken cancellationToken)
        {
            var query = _diamondShapeRepository.GetQuery();
            return query.ToList();
            //throw new NotImplementedException();
        }
    }
}
