using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Create
{
    public record CreateSizeMetalCommand(JewelryModelId ModelId, List<ModelMetalSizeRequestDto> MetalSizeSpecs) : IRequest<Result>;
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
            request.Deconstruct(out JewelryModelId modelId, out List<ModelMetalSizeRequestDto> metalSizeSpecs);
            List<SizeMetal> sizeMetals = metalSizeSpecs.Select(p => SizeMetal.Create(modelId, MetalId.Parse(p.MetalId), SizeId.Parse(p.SizeId), p.Weight)).ToList();
            await _sizeMetalRepository.CreateRange(sizeMetals, token);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Ok();
        }
    }
}
