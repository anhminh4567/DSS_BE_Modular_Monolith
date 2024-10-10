using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.MainDiamonds.Commands.CompareDiamondShape
{
    public record CompareDiamondShapeCommand(JewelryModelId JewelryModelId, List<Diamond> Diamonds) : IRequest<Result>;
    internal class CompareDiamondShapeCommandHandler : IRequestHandler<CompareDiamondShapeCommand, Result>
    {
        private readonly IMainDiamondRepository _mainDiamondRepository;

        public CompareDiamondShapeCommandHandler(
            IMainDiamondRepository mainDiamondRepository)
        {
            _mainDiamondRepository = mainDiamondRepository;
        }

        public async Task<Result> Handle(CompareDiamondShapeCommand request, CancellationToken token)
        {
            request.Deconstruct(out JewelryModelId jewelryModelId, out List <Diamond> diamonds);
            var diamondReqs = await _mainDiamondRepository.GetCriteria(jewelryModelId);
            if (diamonds.Count != diamondReqs.Sum(p => p.Quantity))
                return Result.Fail(new ConflictError("The quantity of the main diamond differs from what the model requires."));

            var flagUnmatchedDiamonds = MatchingDiamond(diamonds, diamondReqs);
            if (flagUnmatchedDiamonds) return Result.Fail(new ConflictError(""));
            return Result.Ok();
        }
        private bool MatchingDiamond(List<Diamond> diamonds, List<MainDiamondReq> diamondReqs)
        {
            var diamondShapeCaratHolder = diamonds.Select(p => new DiamondShapeCaratHolder(p.DiamondShapeId, p.Carat)).ToList();
            return Backtracking(diamondShapeCaratHolder, diamondReqs);
        }
        private bool Backtracking(List<DiamondShapeCaratHolder> shapes, List<MainDiamondReq> diamondReqs, int index = 0)
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
                        if (matchedShape.CaratFrom >= shape.Carat && matchedShape.CaratTo <= shape.Carat)
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
    }
}
