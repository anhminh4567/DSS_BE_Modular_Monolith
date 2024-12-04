using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;
using System.Data;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Delete
{
    public record DeleteModelSideDiamondCommand(string SideDiamondOptId) : IRequest<Result>;
    internal class DeleteModelSideDiamondCommandHandler : IRequestHandler<DeleteModelSideDiamondCommand, Result>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteModelSideDiamondCommandHandler(IJewelryRepository jewelryRepository, ISideDiamondRepository sideDiamondRepository, IUnitOfWork unitOfWork)
        {
            _jewelryRepository = jewelryRepository;
            _sideDiamondRepository = sideDiamondRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteModelSideDiamondCommand request, CancellationToken token)
        {
            request.Deconstruct(out string sideDiamondOptId);
            await _unitOfWork.BeginTransactionAsync(token);
            var sideDiamond = await _sideDiamondRepository.GetById(SideDiamondOptId.Parse(sideDiamondOptId));
            if (sideDiamond == null)
                return Result.Fail(JewelryModelErrors.SideDiamond.SideDiamondOptNotFoundError);
            var sides = await _sideDiamondRepository.GetByModelId(sideDiamond.ModelId);
            if (sides == null || sides.Count() == 0)
                return Result.Fail(JewelryModelErrors.SideDiamond.ModelUnsupportedError);
            if (sides.Count() == 1)
                return Result.Fail(JewelryModelErrors.SideDiamond.SideDiamondOptMinimumError);
            var inUseFlag = await _jewelryRepository.Existing(sideDiamond.ModelId, sideDiamond);
            if (inUseFlag)
                return Result.Fail(JewelryModelErrors.SideDiamond.SideDiamondOptInUseConflictError);
            await _sideDiamondRepository.Delete(sideDiamond, token);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}
