using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Api.Controllers.JewelryModels.UpdateFee
{
    public record UpdateModelFeeCommand(string ModelId, decimal NewFee) : IRequest<Result<JewelryModel>>;
    internal class UpdateModelFeeCommandHandler : IRequestHandler<UpdateModelFeeCommand, Result<JewelryModel>>
    {
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateModelFeeCommandHandler(IJewelryModelRepository jewelryModelRepository, IUnitOfWork unitOfWork)
        {
            _jewelryModelRepository = jewelryModelRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<JewelryModel>> Handle(UpdateModelFeeCommand request, CancellationToken token)
        {
            request.Deconstruct(out string modelId, out decimal newFee);
            await _unitOfWork.BeginTransactionAsync(token);
            var jewelryModel = await _jewelryModelRepository.GetById(JewelryModelId.Parse(modelId));
            if (jewelryModel == null)
                return Result.Fail("This jewelry model doesn't exist");
            jewelryModel.CraftmanFee = newFee;
            await _jewelryModelRepository.Update(jewelryModel);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return jewelryModel;
        }
    }
}
