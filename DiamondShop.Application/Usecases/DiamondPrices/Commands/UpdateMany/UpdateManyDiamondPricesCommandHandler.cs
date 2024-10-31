using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.UpdateMany
{
    public record UpdatedDiamondPrice(string diamondCriteriaId, decimal price); 
    public record UpdateManyDiamondPricesCommand(List<UpdatedDiamondPrice> updatedDiamondPrices, string shapeId, bool islabDiamond, bool? IsSideDiamond = false) :  IRequest<Result<List<DiamondPrice>>>;
    internal class UpdateManyDiamondPricesCommandHandler : IRequestHandler<UpdateManyDiamondPricesCommand, Result<List<DiamondPrice>>>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public UpdateManyDiamondPricesCommandHandler(IDiamondPriceRepository diamondPriceRepository, IUnitOfWork unitOfWork, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondShapeRepository diamondShapeRepository)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _unitOfWork = unitOfWork;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondShapeRepository = diamondShapeRepository;
        }

        public async Task<Result<List<DiamondPrice>>> Handle(UpdateManyDiamondPricesCommand request, CancellationToken cancellationToken)
        {
            var parsedShapeId = DiamondShapeId.Parse(request.shapeId);
            var getShape = await _diamondShapeRepository.GetById(parsedShapeId);
            if (getShape == null)
                return Result.Fail(new NotFoundError());
            if (getShape.Id != DiamondShape.ROUND.Id && getShape.Id != DiamondShape.FANCY_SHAPES.Id)
                return Result.Fail("the shape for price can only be Round brilliant or Fancy, which is round and the rest of the shape");
            List<(DiamondCriteriaId criteriaId, decimal normalizedPrice)> parsedList =
                request.updatedDiamondPrices
                .Select(x => (DiamondCriteriaId.Parse(x.diamondCriteriaId), MoneyVndRoundUpRules.RoundAmountFromDecimal(x.price))
                ).ToList();
            List<DiamondCriteriaId> diamondCriteriaIds = parsedList.Select(x => x.criteriaId).ToList();
            List<DiamondPrice> getPrices = new();

            if(request.IsSideDiamond is false)
                getPrices = await _diamondPriceRepository.GetPriceByShapes(getShape,request.islabDiamond,cancellationToken);
            else
                getPrices = await _diamondPriceRepository.GetSideDiamondPrice( request.islabDiamond, cancellationToken);

            var getPriceByCriteria = getPrices.Where(x => diamondCriteriaIds.Contains(x.CriteriaId)).ToList();
            foreach (var price in getPriceByCriteria)
            {
                var updatedPrice = parsedList.FirstOrDefault(x => x.criteriaId == price.CriteriaId);
                price.ChangePrice(updatedPrice.normalizedPrice);
                await _diamondPriceRepository.Update(price);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return getPrices;
        }
    }
}
