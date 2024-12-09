using DiamondShop.Application.Dtos.Requests.Diamonds;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ErrorMessages;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ErrorMessages;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.CreateMany
{
    //bool isFancyShapePrice
    public record CreateManyDiamondPricesCommand(List<DiamondPriceRequestDto> listPrices, string shapeId, bool IsLabDiamond, bool IsSideDiamond) : IRequest<Result>;
    public class CreateManyDiamondPricesCommandHandler : IRequestHandler<CreateManyDiamondPricesCommand, Result>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateManyDiamondPricesCommandHandler(IDiamondPriceRepository diamondPriceRepository, IDiamondShapeRepository diamondShapeRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IUnitOfWork unitOfWork)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CreateManyDiamondPricesCommand request, CancellationToken cancellationToken)
        {
            DiamondShape correctShape;
            List<DiamondCriteria> getCriteria;
            var parsedShapeId = DiamondShapeId.Parse(request.shapeId);
            var getShapes = await _diamondShapeRepository.GetAllIncludeSpecialShape();
            correctShape = getShapes.FirstOrDefault(s => s.Id == parsedShapeId);
            if (request.IsSideDiamond)
                correctShape = getShapes.FirstOrDefault(s => s.Id == DiamondShape.ANY_SHAPES.Id);
            if (correctShape is null)
                return Result.Fail(DiamondShapeErrors.NotFoundError);


            await _unitOfWork.BeginTransactionAsync();


            if (correctShape is null)
                return Result.Fail(new NotFoundError());

            var mappedListPrice = request.listPrices.Select(x => new { CriteriaId = DiamondCriteriaId.Parse(x.DiamondCriteriaId), Price = MoneyVndRoundUpRules.RoundAmountFromDecimal(x.price), cut = x.cut, color = x.Color, clarity = x.Clarity });
            var mappedCriteria = mappedListPrice.Select(x => x.CriteriaId).ToList();
            getCriteria = await _diamondCriteriaRepository.GetCriteriasByManyId(mappedCriteria);
            List<DiamondCriteriaId> checkCriteriaIds = new();
            foreach (var price in mappedListPrice)
            {
                if (request.IsSideDiamond == false)
                {
                    var tryGetCriteria = getCriteria.FirstOrDefault(c => c.Id == price.CriteriaId && c.IsSideDiamond == false);
                    if (tryGetCriteria is null)
                        return Result.Fail(DiamondPriceErrors.DiamondCriteriaErrors.NotFoundError);
                    DiamondPrice newPrice = null;
                    if (correctShape.IsFancy())
                        newPrice = DiamondPrice.Create(correctShape.Id, tryGetCriteria.Id, price.Price, request.IsLabDiamond, null, price.color, price.clarity);
                    else
                    {
                        //if (price.cut == null)
                        //    throw new Exception("round cần có cut , shape còn lại thì ko");
                        newPrice = DiamondPrice.Create(correctShape.Id, tryGetCriteria.Id, price.Price, request.IsLabDiamond, null, price.color, price.clarity);
                    }
                    checkCriteriaIds.Add(tryGetCriteria.Id);
                    await _diamondPriceRepository.Create(newPrice);
                }
                else
                {
                    var tryGetCriteria = getCriteria.FirstOrDefault(c => c.Id == price.CriteriaId && c.IsSideDiamond == true);
                    if (tryGetCriteria is null)
                        return Result.Fail(DiamondPriceErrors.DiamondCriteriaErrors.NotFoundError);
                    var newPrice = DiamondPrice.CreateSideDiamondPrice(tryGetCriteria.Id, price.Price, request.IsLabDiamond, correctShape, null, price.color, price.clarity);
                    checkCriteriaIds.Add(tryGetCriteria.Id);
                    await _diamondPriceRepository.Create(newPrice);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            _diamondPriceRepository.RemoveAllCache();
            //_diamondPriceRepository.ExecuteUpdateCriteriaUpdateTime(checkCriteriaIds.ToArray());
            return Result.Ok();
        }
    }
}
