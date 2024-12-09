using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ErrorMessages;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
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
    public record UpdateDiamondCriteriaRangeCommand(bool isSideDiamond, string diamondShapeId, CaratRange oldCaratRange, CaratRange newCaratRange) : IRequest<Result<List<DiamondCriteria>>>;
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
            //Dictionary<Cut, Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>>> groupedByCut = new();
            //Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>> allCriteria  = new();
            //(float CaratFrom, float CaratTo)? tobeUpdatedRange = null;
            var parsedShapeId = DiamondShapeId.Parse(request.diamondShapeId);
            var getshape = getAllShape.FirstOrDefault(x => x.Id == parsedShapeId);

            if (request.isSideDiamond == false)
            {
                if (getshape is null)
                    return Result.Fail(DiamondShapeErrors.NotFoundError);
                bool isfancyshape = DiamondShape.IsFancyShape(getshape.Id);
                //if (isfancyshape)
                //{
                Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>> allCriteria = new();
                (float CaratFrom, float CaratTo)? tobeUpdatedRange = null;

                allCriteria = await _diamondCriteriaRepository.GroupAllAvailableCriteria(getshape, cancellationToken);
                var orderedCutRange = allCriteria.Keys.OrderBy(x => x.CaratFrom).ToList();
                foreach (var range in orderedCutRange)
                {
                    if (request.oldCaratRange.caratFrom == range.CaratFrom
                        && request.oldCaratRange.caratTo == range.CaratTo)
                    {
                        tobeUpdatedRange = range;
                    }
                }
                if (tobeUpdatedRange == null)
                    return Result.Fail(DiamondPriceErrors.DiamondCriteriaErrors.CaratRangeNotExist);
                foreach (var range in orderedCutRange)
                {
                    if (tobeUpdatedRange?.CaratFrom == range.CaratFrom && tobeUpdatedRange?.CaratTo == range.CaratTo)
                    {
                        continue;
                    }
                    if (request.newCaratRange.caratFrom <= range.CaratTo && request.newCaratRange.caratTo >= range.CaratFrom)
                    {
                        return Result.Fail(DiamondPriceErrors.DiamondCriteriaErrors.CaratRangeOverlaps(range.CaratFrom, range.CaratTo));
                    }
                }
                var getCriteriasFromCutGroup = allCriteria[tobeUpdatedRange.Value];
                if (getCriteriasFromCutGroup == null)
                    throw new Exception("at this point this should not be null at all");
                getCriteriasFromCutGroup.ForEach(x => 
                {
                    x.ChangeCaratRange(request.newCaratRange.caratFrom, request.newCaratRange.caratTo);
                    _diamondCriteriaRepository.Update(x).Wait();
                });
                await _unitOfWork.SaveChangesAsync();
                _diamondPriceRepository.RemoveAllCache();
                return allCriteria.Values
                    .SelectMany(x => x)
                    .ToList();
                //}
                //else
                //{
                //Dictionary<Cut, Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>>> groupedByCut = new();
                //foreach (var cut in CutHelper.GetCutList())
                //{
                //    Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>> allCriteria = new();
                //    (float CaratFrom, float CaratTo)? tobeUpdatedRange = null;

                //    allCriteria = await _diamondCriteriaRepository.GroupAllAvailableCriteria(getshape, cancellationToken);
                //    groupedByCut.Add(cut, allCriteria);
                //    var orderedCutRange = allCriteria.Keys.OrderBy(x => x.CaratFrom).ToList();
                //    foreach (var range in orderedCutRange)
                //    {
                //        if (request.oldCaratRange.caratFrom == range.CaratFrom
                //            && request.oldCaratRange.caratTo == range.CaratTo)
                //        {
                //            tobeUpdatedRange = range;
                //        }
                //    }
                //    if (tobeUpdatedRange == null)
                //        return Result.Fail(DiamondPriceErrors.DiamondCriteriaErrors.CaratRangeNotExist);
                //    foreach (var range in orderedCutRange)
                //    {
                //        if (tobeUpdatedRange?.CaratFrom == range.CaratFrom && tobeUpdatedRange?.CaratTo == range.CaratTo)
                //        {
                //            continue; //ignore the old range, since this is what we aim to update
                //        }
                //        if (request.newCaratRange.caratFrom <= range.CaratTo && request.newCaratRange.caratTo >= range.CaratFrom)
                //        {
                //            return Result.Fail(DiamondPriceErrors.DiamondCriteriaErrors.CaratRangeOverlaps(range.CaratFrom,range.CaratTo));
                //        }
                //    }
                //    var getCriteriasFromCutGroup = allCriteria[tobeUpdatedRange.Value];
                //    if (getCriteriasFromCutGroup == null)
                //        throw new Exception("at this point this should not be null at all");
                //    getCriteriasFromCutGroup.ForEach(x => x.ChangeCaratRange(request.newCaratRange.caratFrom, request.newCaratRange.caratTo));
                //}
                //await _unitOfWork.SaveChangesAsync();
                //_diamondPriceRepository.RemoveAllCache();
                //return groupedByCut.Values
                //    .SelectMany(x => x.Values)
                //    .SelectMany(x => x)
                //    .ToList();
                //}
            }
            else
            {
                Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>> allCriteria = new();
                (float CaratFrom, float CaratTo)? tobeUpdatedRange = null;
                allCriteria = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCriteria(cancellationToken);
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
                    return Result.Fail(DiamondPriceErrors.DiamondCriteriaErrors.CaratRangeNotExist);
                foreach (var range in orderedRange)
                {
                    if (tobeUpdatedRange?.CaratFrom == range.CaratFrom && tobeUpdatedRange?.CaratTo == range.CaratTo)
                    {
                        continue; //ignore the old range, since this is what we aim to update
                    }
                    if (newRange.caratFrom < range.CaratTo && newRange.caratTo > range.CaratFrom)
                    {
                        return Result.Fail(DiamondPriceErrors.DiamondCriteriaErrors.CaratRangeOverlaps(range.CaratFrom, range.CaratTo));
                    }
                }
                var getCriteriasFromGroup = allCriteria[tobeUpdatedRange.Value];
                if (getCriteriasFromGroup == null)
                    throw new Exception("at this point this should not be null at all");
                getCriteriasFromGroup.ForEach(x => x.ChangeCaratRange(newRange.caratFrom, newRange.caratTo));

                await _unitOfWork.SaveChangesAsync();
                _diamondPriceRepository.RemoveAllCache();
                return getCriteriasFromGroup;
            }
        }
    }
}
