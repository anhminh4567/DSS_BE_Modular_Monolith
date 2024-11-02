using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Commands.UpdateRange
{
    public record CaratRange(float caratFrom, float caratTo);
    public record UpdateDiamondCriteriaRangeCommand(bool isSideDiamond, CaratRange oldCaratRange, CaratRange newCaratRange) : IRequest<Result<List<DiamondCriteria>>>;
    internal class UpdateDiamondCriteriaRangeCommandHandler : IRequestHandler<UpdateDiamondCriteriaRangeCommand, Result<List<DiamondCriteria>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public UpdateDiamondCriteriaRangeCommandHandler(IUnitOfWork unitOfWork, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondPriceRepository diamondPriceRepository, IDiamondShapeRepository diamondShapeRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondShapeRepository = diamondShapeRepository;
        }

        public async Task<Result<List<DiamondCriteria>>> Handle(UpdateDiamondCriteriaRangeCommand request, CancellationToken cancellationToken)
        {
            var getAllShape = await _diamondShapeRepository.GetAllIncludeSpecialShape();
            //List<(float CaratFrom, float CaratTo)> allAvailableCaratRange = new();
            Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>> allCriteria  = new();
            (float CaratFrom, float CaratTo)? tobeUpdatedRange = null;
            if (request.isSideDiamond == false)
            {
                allCriteria = await _diamondCriteriaRepository.GroupAllAvailableCriteria(cancellationToken);
            }
            //allAvailableCaratRange = await _diamondCriteriaRepository.GroupAllAvailableCaratRange(cancellationToken);
            else 
            {
                allCriteria = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCriteria(cancellationToken);
            }
            //allAvailableCaratRange = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCaratRange(cancellationToken);
            var orderedRange = allCriteria.Keys.OrderBy(x => x.CaratFrom).ToList();
            var oldRange = request.oldCaratRange;
            var newRange = request.newCaratRange;
            //get the tobe updated range
            foreach (var range in orderedRange)
            {
                if (oldRange.caratFrom == range.CaratFrom
                    && oldRange.caratTo == range.CaratTo)
                {
                    tobeUpdatedRange = range;
                }
            }
            if (tobeUpdatedRange == null)
                return Result.Fail("The given range does not exist in the database");
            foreach (var range in orderedRange)
            {
                if (tobeUpdatedRange?.CaratFrom == range.CaratFrom && tobeUpdatedRange?.CaratTo == range.CaratTo)
                {
                    continue; //ignore the old range, since this is what we aim to update
                }
                if (newRange.caratFrom < range.CaratTo && newRange.caratTo > range.CaratFrom)
                {
                    return Result.Fail("The given range already exists or overlaps with an existing range in the database");
                }
            }
            var getCriteriasFromGroup = allCriteria[tobeUpdatedRange.Value];
            if (getCriteriasFromGroup == null)
                throw new Exception("at this point this should not be null at all");
            getCriteriasFromGroup.ForEach(x => x.ChangeCaratRange(newRange.caratFrom, newRange.caratTo));
            await _unitOfWork.SaveChangesAsync();
            return getCriteriasFromGroup;
        }
    }
}
