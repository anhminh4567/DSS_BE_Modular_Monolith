using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using DiamondShop.Commons;
using DiamondShop.Application.Services.Interfaces;

namespace DiamondShop.Application.Usecases.DiamondShapes.Commands.Update
{
    public record UpdateDiamondShapeCommand(string shapeName, string id) : IRequest<Result<DiamondShape>>;
    internal class UpdateDiamondShapeCommandHandler : IRequestHandler<UpdateDiamondShapeCommand, Result<DiamondShape>>
    {
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDiamondShapeCommandHandler(IDiamondShapeRepository diamondShapeRepository, IUnitOfWork unitOfWork)
        {
            _diamondShapeRepository = diamondShapeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DiamondShape>> Handle(UpdateDiamondShapeCommand request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string shapeName, out string givenId);
            var id = DiamondShapeId.Parse(givenId);
            var get = await _diamondShapeRepository.GetById(givenId);
            if(get == null) 
            {
                return Result.Fail(new NotFoundError("no shape found for this id")) ;
            }
            get.Update(shapeName);
            await _diamondShapeRepository.Update(get);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(get);
        }
    }
}
