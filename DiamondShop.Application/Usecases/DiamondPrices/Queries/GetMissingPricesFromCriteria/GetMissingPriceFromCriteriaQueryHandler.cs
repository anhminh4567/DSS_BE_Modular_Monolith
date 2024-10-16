using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Queries.GetMissingPricesFromCriteria
{
    public record GetMissingPriceFromCriteriaQuery(string criteriadId) : IRequest<Result<List<DiamondShape>>>;
    internal class GetMissingPriceFromCriteriaQueryHandler : IRequestHandler<GetMissingPriceFromCriteriaQuery, Result<List<DiamondShape>>>
    {
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;

        public GetMissingPriceFromCriteriaQueryHandler(IDiamondShapeRepository diamondShapeRepository, IDiamondPriceRepository diamondPriceRepository, IDiamondCriteriaRepository diamondCriteriaRepository)
        {
            _diamondShapeRepository = diamondShapeRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
        }

        public async Task<Result<List<DiamondShape>>> Handle(GetMissingPriceFromCriteriaQuery request, CancellationToken cancellationToken)
        {
            var parsedId = DiamondCriteriaId.Parse(request.criteriadId);
            var getCriteria = await _diamondCriteriaRepository.GetById(parsedId);
            if (getCriteria == null)
                return Result.Fail(new NotFoundError()) ;
            var getShapes = await _diamondShapeRepository.GetAll();
            var getPrices = await _diamondPriceRepository.GetPriceByCriteria(parsedId);
            IEnumerable<DiamondShape> missingShapes = getShapes.Where(s => !getPrices.Any(p => p.ShapeId == s.Id));
            return missingShapes.ToList();
        }
    }
}
