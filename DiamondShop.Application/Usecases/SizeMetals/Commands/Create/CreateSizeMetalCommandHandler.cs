using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Create
{
    public record ModelMetalSizeSpec(string MetalId, string SizeId, float Weight);
    public record CreateSizeMetalCommand(JewelryModelId ModelId, List<ModelMetalSizeSpec> MetalSizeSpecs) : IRequest<Result>;
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
            request.Deconstruct(out JewelryModelId modelId, out List<ModelMetalSizeSpec> metalSizeSpecs);
            List<SizeMetal> sizeMetals = metalSizeSpecs.Select(p => SizeMetal.Create(modelId, MetalId.Parse(p.MetalId), SizeId.Parse(p.SizeId), p.Weight)).ToList();
            await _sizeMetalRepository.CreateRange(sizeMetals, token);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Ok();
        }
    }
}
