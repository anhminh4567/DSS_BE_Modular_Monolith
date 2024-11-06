using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Update
{
    public record UpdateModelSizeMetalCommand(string ModelId, List<ModelMetalSizeRequestDto> SizeMetals) : IRequest<Result<List<SizeMetal>>>;
    internal class UpdateModelSizeMetalCommandHandler : IRequestHandler<UpdateModelSizeMetalCommand, Result<List<SizeMetal>>>
    {
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateModelSizeMetalCommandHandler(ISizeMetalRepository sizeMetalRepository, IUnitOfWork unitOfWork)
        {
            _sizeMetalRepository = sizeMetalRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<SizeMetal>>> Handle(UpdateModelSizeMetalCommand request, CancellationToken token)
        {
            request.Deconstruct(out string modelId, out List<ModelMetalSizeRequestDto> sizeMetalUpdates);
            await _unitOfWork.BeginTransactionAsync(token);
            var sizeMetalQuery = _sizeMetalRepository.GetQuery();
            sizeMetalQuery = _sizeMetalRepository.QueryFilter(sizeMetalQuery, p => p.ModelId == JewelryModelId.Parse(modelId));
            var sizeMetals = sizeMetalQuery.ToList();
            if (sizeMetals == null)
                return Result.Fail("The size and metal selection for this model doesn't exist");
            sizeMetals.ForEach(p =>
            {
                var item = sizeMetalUpdates.FirstOrDefault(k => k.MetalId == p.MetalId.Value && k.SizeId == p.SizeId.Value);
                if (item != null)
                    p.Weight = item.Weight;
            });
            _sizeMetalRepository.UpdateRange(sizeMetals, token);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return sizeMetals;
        }
    }
}
