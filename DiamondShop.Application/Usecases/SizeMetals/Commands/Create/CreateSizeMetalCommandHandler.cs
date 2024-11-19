using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Create
{
    public record CreateSizeMetalCommand(string ModelId, ModelMetalSizeRequestDto MetalSizeSpec) : IRequest<Result>;
    internal class CreateSizeMetalCommandHandler : IRequestHandler<CreateSizeMetalCommand, Result>
    {
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateSizeMetalCommandHandler(ISizeMetalRepository sizeMetalRepository, IUnitOfWork unitOfWork)
        {
            _sizeMetalRepository = sizeMetalRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CreateSizeMetalCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out string modelId, out ModelMetalSizeRequestDto metalSizeSpec);
            var sizeMetal = SizeMetal.Create(JewelryModelId.Parse(modelId), MetalId.Parse(metalSizeSpec.MetalId), SizeId.Parse(metalSizeSpec.SizeId), metalSizeSpec.Weight);
            var existFlag = await _sizeMetalRepository.Existing(sizeMetal.ModelId, sizeMetal.MetalId, sizeMetal.SizeId);
            if (existFlag)
                return Result.Fail(JewelryModelErrors.SizeMetal.SizeMetalNotFoundError);
            await _sizeMetalRepository.Create(sizeMetal, token);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Ok();
        }
    }
}
