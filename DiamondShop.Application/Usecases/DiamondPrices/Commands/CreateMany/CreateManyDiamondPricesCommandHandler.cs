using DiamondShop.Application.Dtos.Requests.Diamonds;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.CreateMany
{
    public record CreateManyDiamondPricesCommand(List<DiamondPriceRequestDto> listPrices, bool IsSideDiamondPrices = false) : IRequest<Result>;
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
            var getShapes = await _diamondShapeRepository.GetAllIncludeSpecialShape();
            var getCriteria = await _diamondCriteriaRepository.GetAll();
            await _unitOfWork.BeginTransactionAsync();
            foreach (var price in request.listPrices)
            {

                var tryGetShape = getShapes.FirstOrDefault(s => s.Id == price.DiamondShapeId);
                if(tryGetShape is null)
                    return Result.Fail(new NotFoundError());
                if(request.IsSideDiamondPrices == false)
                {
                    var tryGetCriteria = getCriteria.FirstOrDefault(c => c.Id == price.DiamondCriteriaId && c.IsSideDiamond == false);
                    if (getCriteria is null)
                        return Result.Fail(new NotFoundError());
                    var newPrice = DiamondPrice.Create(tryGetShape.Id, tryGetCriteria.Id, price.price, price.IsLabDiamond);
                    await _diamondPriceRepository.Create(newPrice);
                }
                else
                {
                    var tryGetCriteria = getCriteria.FirstOrDefault(c => c.Id == price.DiamondCriteriaId && c.IsSideDiamond == true);
                    if (getCriteria is null)
                        return Result.Fail(new NotFoundError());
                    var newPrice = DiamondPrice.CreateSideDiamondPrice(tryGetCriteria.Id, price.price, price.IsLabDiamond);
                    await _diamondPriceRepository.Create(newPrice);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return Result.Ok();
        }
    }
}
