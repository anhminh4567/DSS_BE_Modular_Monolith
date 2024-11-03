using DiamondShop.Commons;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
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
        public async Task<Result> CheckMatchingDiamond(JewelryModelId jewelryModelId, List<Diamond> diamonds, IMainDiamondRepository mainDiamondRepository)
        {
            var diamondReqs = await mainDiamondRepository.GetCriteria(jewelryModelId);
            if (diamonds.Count != diamondReqs.Sum(p => p.Quantity))
                return Result.Fail(new ConflictError("The quantity of the main diamond differs from what the model requires."));
            var flagMatchedDiamonds = MatchingDiamond(diamonds, diamondReqs.ToList());
            if (!flagMatchedDiamonds) return Result.Fail(new ConflictError("Diamonds don't meet the model requirement."));
            return Result.Ok();
        }
        private bool MatchingDiamond(List<Diamond> diamonds, List<MainDiamondReq> diamondReqs)
        {
            var diamondShapeCaratHolder = diamonds.Select(p => new DiamondShapeHolder(p.DiamondShapeId, p.Carat)).ToList();
            return Backtracking(diamondShapeCaratHolder, diamondReqs);
        }
        private bool Backtracking(List<DiamondShapeHolder> shapes, List<MainDiamondReq> diamondReqs, int index = 0)
        {
            if (index == shapes.Count) return true;
            var shape = shapes[index];
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
                            if (Backtracking(shapes, diamondReqs, ++index))
                            {
                                return true;
                            }
                            req.Quantity++;
                        }
                    }
                }
            }
            return false;
        }


        public async Task<Result> CheckMatchingDiamond(JewelryModelId jewelryModelId, List<DiamondRequest> customizeRequests, IMainDiamondRepository mainDiamondRepository)
        {
            var diamondReqs = await mainDiamondRepository.GetCriteria(jewelryModelId);
            if (customizeRequests.Count != diamondReqs.Sum(p => p.Quantity))
                return Result.Fail(new ConflictError("The quantity of the main diamond differs from what the model requires."));
            var ordered = customizeRequests.OrderBy(p => p.DiamondShapeId == null).ThenBy(p => p.DiamondShape).ToList();
            var flagMatchedDiamonds = MatchingDiamond(ordered, diamondReqs);
            if (!flagMatchedDiamonds) return Result.Fail(new ConflictError("Diamonds don't meet the model requirement."));
            return Result.Ok();
        }
        private bool MatchingDiamond(List<DiamondRequest> diamonds, List<MainDiamondReq> diamondReqs)
        {
            var diamondShapeCaratHolder = diamonds.Select(p => new CustomizeShapeHolder(p.DiamondShapeId, p.CaratFrom, p.CaratTo)).ToList();
            return Backtracking(diamondShapeCaratHolder, diamondReqs);
        }
        private bool Backtracking(List<CustomizeShapeHolder> shapes, List<MainDiamondReq> diamondReqs, int index = 0)
        {
            if (index == shapes.Count) return true;
            var shape = shapes[index];
            foreach (var req in diamondReqs)
            {
                if (req.Quantity > 0)
                {
                    var matchedShape = req.Shapes.FirstOrDefault(p => {
                        if (shape.ShapeId == null)
                            return true;
                        return p.ShapeId == shape.ShapeId; 
                    });
                    if (matchedShape != null)
                    {
                        if (matchedShape.CaratFrom >= shape.CaratFrom && matchedShape.CaratTo <= shape.CaratTo)
                        {
                            req.Quantity--;
                            //Sending a new list of main diamond request with the current removed so that nullable diamondShape will always get a non conflicting one
                            var newList = diamondReqs.ToList();
                            newList.Remove(req);
                            if (Backtracking(shapes, newList, ++index))
                            {
                                return true;
                            }
                            req.Quantity++;
                        }
                    }
                }
            }
            return false;
        }
    }
}
