using DiamondShop.Commons;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    record DiamondShapeHolder(DiamondShapeId ShapeId, float Carat);
    record CustomizeShapeHolder(DiamondShapeId? ShapeId, float? CaratFrom, float? CaratTo);
    public class MainDiamondService : IMainDiamondService
    {
        private readonly IMainDiamondRepository _mainDiamondRepository;

        public MainDiamondService(IMainDiamondRepository mainDiamondRepository)
        {
            _mainDiamondRepository = mainDiamondRepository;
        }

        public async Task<Result> CheckMatchingDiamond(JewelryModelId jewelryModelId, List<Diamond> diamonds)
        {
            var diamondReqs = await _mainDiamondRepository.GetCriteria(jewelryModelId);
            if (diamonds.Count != diamondReqs.Sum(p => p.Quantity))
                return Result.Fail(JewelryModelErrors.MainDiamond.MainDiamondCountError(diamondReqs.Sum(p => p.Quantity)));
            var flagMatchedDiamonds = MatchingDiamond(diamonds, diamondReqs.ToList());
            return flagMatchedDiamonds;
        }
        private Result MatchingDiamond(List<Diamond> diamonds, List<MainDiamondReq> diamondReqs)
        {
            var diamondShapeCaratHolder = diamonds.Select(p => new DiamondShapeHolder(p.DiamondShapeId, p.Carat)).ToList();
            return Backtracking(diamondShapeCaratHolder, diamondReqs);
        }
        private Result Backtracking(List<DiamondShapeHolder> shapes, List<MainDiamondReq> diamondReqs, int index = 0)
        {
            if (index == shapes.Count) return Result.Ok();
            var shape = shapes[index];
            List<IError> errors = new();
            foreach (var req in diamondReqs)
            {
                if (req.Quantity > 0)
                {
                    var matchedShape = req.Shapes.FirstOrDefault(p => p.ShapeId == shape.ShapeId);
                    if (matchedShape != null)
                    {
                        if (matchedShape.CaratFrom <= shape.Carat && matchedShape.CaratTo >= shape.Carat)
                        {
                            req.Quantity--;
                            if (Backtracking(shapes, diamondReqs, ++index).IsSuccess)
                            {
                                return Result.Ok();
                            }
                            req.Quantity++;
                        }
                        else
                            errors.Add(JewelryModelErrors.MainDiamond.MismatchCaratError(index+1));
                    }
                    else
                        errors.Add(JewelryModelErrors.MainDiamond.MismatchShapeError(index+1));
                }
            }
            return Result.Fail(errors);
        }


        public async Task<Result> CheckMatchingDiamond(JewelryModelId jewelryModelId, List<DiamondRequest> customizeRequests)
        {
            var diamondReqs = await _mainDiamondRepository.GetCriteria(jewelryModelId);
            if (customizeRequests.Count != diamondReqs.Sum(p => p.Quantity))
                return Result.Fail(JewelryModelErrors.MainDiamond.MainDiamondCountError(diamondReqs.Sum(p => p.Quantity)));
            var ordered = customizeRequests.OrderBy(p => p.DiamondShapeId == null).ThenBy(p => p.DiamondShape).ToList();
            var flagMatchedDiamonds = MatchingDiamond(ordered, diamondReqs);
            return flagMatchedDiamonds;
        }
        private Result MatchingDiamond(List<DiamondRequest> diamonds, List<MainDiamondReq> diamondReqs)
        {
            var diamondShapeCaratHolder = diamonds.Select(p => new CustomizeShapeHolder(p.DiamondShapeId, p.CaratFrom, p.CaratTo)).ToList();
            return Backtracking(diamondShapeCaratHolder, diamondReqs);
        }
        private Result Backtracking(List<CustomizeShapeHolder> shapes, List<MainDiamondReq> diamondReqs, int index = 0)
        {
            if (index == shapes.Count) return Result.Ok();
            var shape = shapes[index];
            List<IError> errors = new();
            foreach (var req in diamondReqs)
            {
                if (req.Quantity > 0)
                {
                    var matchedShape = req.Shapes.FirstOrDefault(p =>
                    {
                        if (shape.ShapeId == null)
                            return true;
                        return p.ShapeId == shape.ShapeId;
                    });
                    if (matchedShape != null)
                    {
                        if (shape.CaratFrom >= matchedShape.CaratFrom && shape.CaratTo <= matchedShape.CaratTo)
                        {
                            req.Quantity--;
                            //Sending a new list of main diamond request with the current removed so that nullable diamondShape will always get a non conflicting one
                            var newList = diamondReqs.ToList();
                            newList.Remove(req);
                            if (Backtracking(shapes, newList, ++index).IsSuccess)
                            {
                                return Result.Ok();
                            }
                            req.Quantity++;
                        }
                        else
                            errors.Add(JewelryModelErrors.MainDiamond.MismatchCaratError(index+1));
                    }
                    else
                        errors.Add(JewelryModelErrors.MainDiamond.MismatchShapeError(index+1));
                }
            }
            return Result.Fail(errors);
        }
    }
}
