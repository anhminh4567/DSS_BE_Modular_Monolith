using Azure.Core;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Commands.DeleteRange
{
    public class DeleteCriteriaByRangeCommandValidator : AbstractValidator<DeleteCriteriaByRangeCommand>
    {
        public DeleteCriteriaByRangeCommandValidator()
        {
            RuleFor(x => x.isSideDiamond).NotNull();
            RuleFor(x => x.caratFrom).Cascade(CascadeMode.Stop).NotNull().GreaterThanOrEqualTo(0);
            RuleFor(x => x.caratFrom).Cascade(CascadeMode.Stop).NotNull().GreaterThanOrEqualTo(0);
            When(x => x.caratFrom != null && x.caratTo != null, () =>
            {
                RuleFor(x => x.caratFrom).LessThan(x => x.caratTo);
            });
        }
    }
    public record DeleteCriteriaByRangeCommand(bool isSideDiamond, float caratFrom, float caratTo, string diamondShapeId) : IRequest<Result>;
    internal class DeleteCriteriaByRangeCommandHandler : IRequestHandler<DeleteCriteriaByRangeCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public DeleteCriteriaByRangeCommandHandler(IUnitOfWork unitOfWork, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondPriceRepository diamondPriceRepository, IDiamondShapeRepository diamondShapeRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondShapeRepository = diamondShapeRepository;
        }

        public async Task<Result> Handle(DeleteCriteriaByRangeCommand request, CancellationToken cancellationToken)
        {
            var pareseShapeId = DiamondShapeId.Parse(request.diamondShapeId);
            var getAllShape = await _diamondShapeRepository.GetAllIncludeSpecialShape();
            var getShape = getAllShape.FirstOrDefault(x => x.Id == pareseShapeId);
            bool isFancyShape = DiamondShape.IsFancyShape(pareseShapeId);
            if (getAllShape is null)
            {
                return Result.Fail("Shape not found");
            }

            Dictionary<Cut, Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>>> groupedByCut = new();
            Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>> getGrooupedCriteria = new();

            if (request.isSideDiamond)
            {
                return await DeleteSideDiamond();
            }
            else
            {
                return await DeleteMainDiamond();
            }


            async Task<Result> DeleteMainDiamond()
            {
                List<KeyValuePair<(float caratFrom, float carat), List<DiamondCriteria>>> listTobeRemoved = new();
                if (isFancyShape)
                {
                    getGrooupedCriteria = await _diamondCriteriaRepository.GroupAllAvailableCriteria(true,null, cancellationToken);
                    Cut anyCut = Cut.Excellent;
                    groupedByCut.Add(anyCut, getGrooupedCriteria);
                    var criteriaInRange = getGrooupedCriteria.FirstOrDefault(x => x.Key.CaratFrom == request.caratFrom && x.Key.CaratTo == request.caratTo);
                    listTobeRemoved.Add(criteriaInRange);
                }
                else
                {
                    foreach (var cut in CutHelper.GetCutList())
                    {
                        getGrooupedCriteria = await _diamondCriteriaRepository.GroupAllAvailableCriteria(false, cut, cancellationToken);
                        groupedByCut.Add(cut, getGrooupedCriteria);
                        var criteriaInRange = getGrooupedCriteria.FirstOrDefault(x => x.Key.CaratFrom == request.caratFrom && x.Key.CaratTo == request.caratTo);
                        listTobeRemoved.Add(criteriaInRange);
                    }
                }

                foreach (var criteriaFromCutGroup in listTobeRemoved)
                {
                    if (criteriaFromCutGroup.Value == null || criteriaFromCutGroup.Value.Count == 0)
                    {
                        return Result.Fail("Criteria not found");
                    }
                    foreach (var crit in criteriaFromCutGroup.Value)
                    {
                        _diamondCriteriaRepository.Delete(crit).Wait();
                    }
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
            async Task<Result> DeleteSideDiamond()
            {
                getGrooupedCriteria = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCriteria(cancellationToken);
                var anyCriteriaInRange = getGrooupedCriteria.FirstOrDefault(x => x.Key.CaratFrom == request.caratFrom && x.Key.CaratTo == request.caratTo);
                if (anyCriteriaInRange.Value == null || anyCriteriaInRange.Value.Count == 0)
                {
                    return Result.Fail("Criteria not found");
                }
                foreach (var criteria in anyCriteriaInRange.Value)
                {
                    _diamondCriteriaRepository.Delete(criteria).Wait();
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
        }

    }
}
