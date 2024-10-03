using DiamondShop.Application.Dtos.Requests.Diamonds;
using DiamondShop.Application.Services.Data;
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
    public record CreateManyDiamondPricesCommand(List<DiamondPriceRequestDto> listPrices) : IRequest<Result>;
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
            var getShapes = await _diamondShapeRepository.GetAll();
            var getCriteria = await _diamondCriteriaRepository.GetAll();
            await _unitOfWork.BeginTransactionAsync();
            foreach (var price in request.listPrices)
            {

                var tryGetShape = getShapes.FirstOrDefault(s => s.Id == price.DiamondShapeId);
                if(tryGetShape is null)
                    return Result.Fail(new NotFoundError());
                var tryGetCriteria = getCriteria.FirstOrDefault(c => c.Id == price.DiamondCriteriaId);
                if (getCriteria is null)
                    return Result.Fail(new NotFoundError());
                var newPrice = DiamondPrice.Create(tryGetShape.Id, tryGetCriteria.Id, price.price);

                await _diamondPriceRepository.Create(newPrice);
                //var tryGet = await _diamondPriceRepository.GetById(price.DiamondShapeId, price.DiamondCriteriaId);
                //if (tryGet != null)
                //    return Result.Fail(new ConflictError("another item with such price (id) exist in db"));
            }
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return Result.Ok();
        }
    }
}
