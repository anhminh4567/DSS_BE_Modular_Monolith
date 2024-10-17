using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.Create
{
    public record CreateDiamondPricesCommand(DiamondCriteriaId DiamondCriteriaId, DiamondShapeId DiamondShapeId, decimal price) : IRequest<Result<DiamondPrice>>;
    internal class CreateDiamondPricesCommandHandler : IRequestHandler<CreateDiamondPricesCommand, Result<DiamondPrice>>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDiamondPricesCommandHandler(IDiamondPriceRepository diamondPriceRepository, IDiamondShapeRepository diamondShapeRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IUnitOfWork unitOfWork)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DiamondPrice>> Handle(CreateDiamondPricesCommand request, CancellationToken cancellationToken)
        {
            var tryGet = await _diamondPriceRepository.GetById(request.DiamondShapeId, request.DiamondCriteriaId);
            if (tryGet != null)
                return Result.Fail(new ConflictError("another item with such price (id) exist in db"));
            var getShape = await _diamondShapeRepository.GetById(request.DiamondShapeId);
            if (getShape is null)
                return Result.Fail(new NotFoundError());
            var getCriteria = await _diamondCriteriaRepository.GetById(request.DiamondCriteriaId);
            if (getCriteria is null)
                return Result.Fail(new NotFoundError());

            var newPrice = DiamondPrice.Create(getShape.Id, getCriteria.Id,request.price);
            await _diamondPriceRepository.Create(newPrice);
            await _unitOfWork.SaveChangesAsync();
            return newPrice;
        }
    }
}
