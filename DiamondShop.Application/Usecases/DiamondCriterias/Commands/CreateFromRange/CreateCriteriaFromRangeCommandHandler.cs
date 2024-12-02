using DiamondShop.Application.Dtos.Requests.Diamonds;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateMany;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateFromRange
{
    public record CreateCriteriaFromRangeCommand(float caratFrom, float caratTo, string diamondShapeId, bool IsSideDiamond = false) : IRequest<Result<List<DiamondCriteria>>>;
    internal class CreateCriteriaFromRangeCommandHandler : IRequestHandler<CreateCriteriaFromRangeCommand, Result<List<DiamondCriteria>>>
    {
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly ISender _sender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public CreateCriteriaFromRangeCommandHandler(IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondShapeRepository diamondShapeRepository, ISender sender, IUnitOfWork unitOfWork, IOptionsMonitor<ApplicationSettingGlobal> options)
        {
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _sender = sender;
            _unitOfWork = unitOfWork;
            _optionsMonitor = options;
        }

        public async Task<Result<List<DiamondCriteria>>> Handle(CreateCriteriaFromRangeCommand request, CancellationToken cancellationToken)
        {
            var diamondPriceRule = _optionsMonitor.CurrentValue.DiamondPriceRules;
            var diamondRule = _optionsMonitor.CurrentValue.DiamondRule;
            List<(float CaratFrom, float CaratTo)> allAvailableCaratRangeForThisShape = new();
            var getAllShape = await _diamondShapeRepository.GetAllIncludeSpecialShape();
            var getShape = getAllShape.FirstOrDefault(s => s.Id == DiamondShapeId.Parse(request.diamondShapeId));

            if (request.IsSideDiamond == false)
            {
                if (getShape is null)
                    return Result.Fail("Shape not found");
                bool isFancyShape = DiamondShape.IsFancyShape(getShape.Id);
                //if(isFancyShape)
                allAvailableCaratRangeForThisShape = await _diamondCriteriaRepository.GroupAllAvailableCaratRange(getShape, cancellationToken);
                //else
                //{
                //    foreach(Cut cut in CutHelper.GetCutList())
                //    {
                //var result = await _diamondCriteriaRepository.GroupAllAvailableCaratRange(getShape, cancellationToken);
                //allAvailableCaratRangeForThisShape.AddRange(result);
                //}
                //}

            }
            else
            {
                allAvailableCaratRangeForThisShape = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCaratRange(cancellationToken);
                getShape = getAllShape.FirstOrDefault(s => s.Id == DiamondShape.ANY_SHAPES.Id);
                if (getShape is null)
                    return Result.Fail("Shape not found");
            }
            var orderedRange = allAvailableCaratRangeForThisShape.OrderBy(x => x.CaratFrom).ToList();
            foreach (var range in orderedRange)
            {
                if (request.IsSideDiamond == false)
                {
                    if (request.caratFrom <= range.CaratTo && request.caratTo >= range.CaratFrom)
                    {
                        return Result.Fail($"The given range already exists or overlaps with an existing range in the database, which is from {range.CaratFrom} to {range.CaratTo}");
                    }
                }
                else
                {
                    if (request.caratFrom < range.CaratTo && request.caratTo > range.CaratFrom)
                    {
                        return Result.Fail("The given range already exists or overlaps with an existing range in the database");
                    }
                }
            }
            // check if valid according to the rule
            if (request.IsSideDiamond)
            {
                if (request.caratTo > diamondRule.BiggestSideDiamondCarat)
                    return Result.Fail("range tối đa của side diammond trong cấu hình là " + diamondRule.BiggestSideDiamondCarat + "carat ");
            }
            else
            {
                if (request.caratFrom < diamondRule.SmallestMainDiamondCarat)
                    return Result.Fail("range smallest của main diammond trong cấu hình là " + diamondRule.SmallestMainDiamondCarat + "carat ");
            }

            //when all is valid
            List<DiamondCriteriaRequestDto> requests = new();
            if (request.IsSideDiamond == false)
            {
                bool isFancyShape = DiamondShape.IsFancyShape(getShape.Id);
                if (isFancyShape)
                {
                    //foreach (var color in Enum.GetValues(typeof(Color)))
                    //{
                    //    foreach (var clarity in Enum.GetValues(typeof(Clarity)))
                    //    {
                    //        requests.Add(new DiamondCriteriaRequestDto()
                    //        {
                    //            CaratFrom = request.caratFrom,
                    //            CaratTo = request.caratTo,
                    //            //Clarity = (Clarity)clarity,
                    //            //Color = (Color)color,
                    //            //Cut = null
                    //        });
                    //    }
                    //}
                    requests.Add(new DiamondCriteriaRequestDto()
                    {
                        CaratFrom = request.caratFrom,
                        CaratTo = request.caratTo,
                        //Clarity = (Clarity)clarity,
                        //Color = (Color)color,
                        //Cut = null
                    });
                }
                else
                {
                    requests.Add(new DiamondCriteriaRequestDto()
                    {
                        CaratFrom = request.caratFrom,
                        CaratTo = request.caratTo,
                        //Clarity = (Clarity)clarity,
                        //Color = (Color)color,
                        //Cut = null//(Cut)cut
                    });
                    ////foreach (var cut in Enum.GetValues(typeof(Cut)))
                    ////{
                    //foreach (var color in Enum.GetValues(typeof(Color)))
                    //{
                    //    foreach (var clarity in Enum.GetValues(typeof(Clarity)))
                    //    {
                    //        requests.Add(new DiamondCriteriaRequestDto()
                    //        {
                    //            CaratFrom = request.caratFrom,
                    //            CaratTo = request.caratTo,
                    //            //Clarity = (Clarity)clarity,
                    //            //Color = (Color)color,
                    //            //Cut = null//(Cut)cut
                    //        });
                    //    }
                    //}
                    ////}
                }
                var command = new CreateManyDiamondCriteriasCommand(requests, getShape.Id.Value, request.IsSideDiamond);
                var result = await _sender.Send(command, cancellationToken);
                return result;
            }
            else if (request.IsSideDiamond)
            {
                requests.Add(new DiamondCriteriaRequestDto()
                {
                    CaratFrom = request.caratFrom,
                    CaratTo = request.caratTo,
                });
                //foreach (var color in Enum.GetValues(typeof(Color)))
                //{
                //    foreach (var clarity in Enum.GetValues(typeof(Clarity)))
                //    {
                //        if (request.IsSideDiamond)
                //        {
                //            requests.Add(new DiamondCriteriaRequestDto()
                //            {
                //                CaratFrom = request.caratFrom,
                //                CaratTo = request.caratTo,
                //                //Clarity = (Clarity)clarity,
                //                //Color = (Color)color,
                //                //Cut = null//request.Cut.Value,
                //            });
                //        }
                //    }
                //}
                var command = new CreateManyDiamondCriteriasCommand(requests, getShape.Id.Value, request.IsSideDiamond);
                var result = await _sender.Send(command, cancellationToken);
                return result;
            }
            else
                return Result.Fail("Unknown whether it is side or main diamond criteria");

        }
    }
}

