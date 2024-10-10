using DiamondShop.Application.Services.Data;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.Create
{
    public record CreateDiamondCommand(Diamond_4C diamond4c, Diamond_Details details, Diamond_Measurement measurement, bool hasGIA, string shapeId, decimal priceOffset = 1) :IRequest<Result<Diamond>>;
    internal class CreateDiamondCommandHandler : IRequestHandler<CreateDiamondCommand, Result<Diamond>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public CreateDiamondCommandHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IDiamondShapeRepository diamondShapeRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _diamondShapeRepository = diamondShapeRepository;
        }

        public async Task<Result<Diamond>> Handle(CreateDiamondCommand request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out var diamond4c, out var details, out var measurement, out bool hasGIA, out string shapeGivenId, out var priceOffset);
            DiamondShapeId shapeId = DiamondShapeId.Parse(shapeGivenId);
            DiamondShape getShape = await _diamondShapeRepository.GetById(shapeId);
            if (getShape is null)
                return Result.Fail(new NotFoundError("no shape found"));
            Diamond newDiamond = Diamond.Create(getShape, diamond4c, details, hasGIA, measurement, priceOffset);
            await _diamondRepository.Create(newDiamond);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(newDiamond);
        }
    }
}
