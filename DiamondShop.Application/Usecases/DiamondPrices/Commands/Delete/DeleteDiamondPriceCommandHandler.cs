using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.Delete
{
    public record DeleteDiamondPriceCommand(string shapeId, string criteriaId) : IRequest<Result>;
    internal class DeleteDiamondPriceCommandHandler : IRequestHandler<DeleteDiamondPriceCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;

        public DeleteDiamondPriceCommandHandler(IUnitOfWork unitOfWork, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondPriceRepository diamondPriceRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondPriceRepository = diamondPriceRepository;
        }

        public async Task<Result> Handle(DeleteDiamondPriceCommand request, CancellationToken cancellationToken)
        {
            var criteriaId = DiamondCriteriaId.Parse(request.criteriaId);
            var shapeId = DiamondShapeId.Parse(request.shapeId);

            var getPrice = await _diamondPriceRepository.GetById(shapeId,criteriaId);
            if (getPrice == null)
                return Result.Fail(new NotFoundError("not found this criteria"));
            await _diamondPriceRepository.Delete(getPrice);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
