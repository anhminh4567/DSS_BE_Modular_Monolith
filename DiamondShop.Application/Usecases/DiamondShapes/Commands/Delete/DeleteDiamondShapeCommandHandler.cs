using DiamondShop.Application.Services.Data;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondShapes.Commands.Delete
{
    public record DeleteDiamondShapeCommand(string id) : IRequest<Result>;
    internal class DeleteDiamondShapeCommandHandler : IRequestHandler<DeleteDiamondShapeCommand,Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public DeleteDiamondShapeCommandHandler(IUnitOfWork unitOfWork, IDiamondShapeRepository repository)
        {
            _unitOfWork = unitOfWork;
            _diamondShapeRepository = repository;
        }

        public async Task<Result> Handle(DeleteDiamondShapeCommand request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string givenId);
            DiamondShapeId id = DiamondShapeId.Parse(givenId);
            var get = await _diamondShapeRepository.GetById(id);
            if (get is null)
                return Result.Fail(new NotFoundError());
            if (await _diamondShapeRepository.IsAnyItemHaveThisShape(id))
                return Result.Fail(new ConflictError("there are items related to this shape, cannot delete"));
            _diamondShapeRepository.Delete(get).Wait();
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
