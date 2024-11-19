using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Delete
{
    public record DeleteModelSizeMetalCommand(string ModelId, string MetalId, string SizeId) : IRequest<Result>;
    internal class DeleteModelSizeMetalCommandHandler : IRequestHandler<DeleteModelSizeMetalCommand, Result>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteModelSizeMetalCommandHandler(IJewelryRepository jewelryRepository, ISizeMetalRepository sizeMetalRepository, IUnitOfWork unitOfWork)
        {
            _jewelryRepository = jewelryRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteModelSizeMetalCommand request, CancellationToken token)
        {
            request.Deconstruct(out string modelId, out string metalId, out string sizeId);
            await _unitOfWork.BeginTransactionAsync(token);
            var sizeMetal = await _sizeMetalRepository.GetById(JewelryModelId.Parse(modelId), MetalId.Parse(metalId), SizeId.Parse(sizeId));
            if (sizeMetal == null)
                return Result.Fail(JewelryModelErrors.SizeMetal.SizeMetalNotFoundError);
            var inUseFlag = await _jewelryRepository.Existing(sizeMetal.ModelId, sizeMetal.MetalId, sizeMetal.SizeId);
            if (inUseFlag)
                return Result.Fail(JewelryModelErrors.SizeMetal.SizeMetalInUseConflictError);
            await _sizeMetalRepository.Delete(sizeMetal, token);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}
