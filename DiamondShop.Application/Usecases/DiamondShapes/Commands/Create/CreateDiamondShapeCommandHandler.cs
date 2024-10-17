using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.DiamondShapes.Commands.Create;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondShapes.Commands.Create
{
    public record CreateDiamondShapeCommand(string shapeName, string? id) : IRequest<Result<DiamondShape>>;
    internal class CreateDiamondShapeCommandHandler : IRequestHandler<CreateDiamondShapeCommand, Result<DiamondShape>>
    {
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDiamondShapeCommandHandler(IDiamondShapeRepository diamondShapeRepository, IUnitOfWork unitOfWork)
        {
            _diamondShapeRepository = diamondShapeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DiamondShape>> Handle(CreateDiamondShapeCommand request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string shapeName, out string? givenId);
            var newShape = givenId is null
                ? DiamondShape.Create(shapeName)
                : DiamondShape.Create(shapeName, DiamondShapeId.Parse(givenId));
            await _diamondShapeRepository.Create(newShape);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(newShape);
        }
    }
}


