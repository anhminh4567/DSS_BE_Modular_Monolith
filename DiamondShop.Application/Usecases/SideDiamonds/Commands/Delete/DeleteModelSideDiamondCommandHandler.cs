using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;

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
                return Result.Fail("This side diamond option for this model doesn't exist");
            var inUseFlag = await _jewelryRepository.Existing(sideDiamond.ModelId, sideDiamond);
            if (inUseFlag)
                return Result.Fail("This side diamond option for this model is still in use");
            await _sideDiamondRepository.Delete(sideDiamond, token);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}
