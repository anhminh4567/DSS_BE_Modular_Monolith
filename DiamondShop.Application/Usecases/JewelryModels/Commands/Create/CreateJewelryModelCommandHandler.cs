using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.MainDiamonds.Commands.Create;
using DiamondShop.Application.Usecases.SideDiamonds.Commands;
using DiamondShop.Application.Usecases.SizeMetals.Commands.Create;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryModels.Commands.Create
{
    public record CreateJewelryModelCommand(JewelryModelRequestDto ModelSpec, List<MainDiamondRequestDto> MainDiamondSpecs, List<SideDiamondRequestDto> SideDiamondSpecs, List<ModelMetalSizeRequestDto> MetalSizeSpecs) : IRequest<Result<JewelryModel>>;
    internal class CreateJewelryModelCommandHandler : IRequestHandler<CreateJewelryModelCommand, Result<JewelryModel>>
    {
        private readonly ISender _sender;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateJewelryModelCommandHandler(
            ISender sender,
            IJewelryModelRepository jewelryModelRepository,
        IUnitOfWork unitOfWork)
        {
            _sender = sender;
            _jewelryModelRepository = jewelryModelRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<JewelryModel>> Handle(CreateJewelryModelCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out JewelryModelRequestDto ModelSpec, out List<MainDiamondRequestDto> MainDiamondSpecs,
                out List<SideDiamondRequestDto> SideDiamondSpecs, out List<ModelMetalSizeRequestDto> MetalSizeSpecs);

            var newModel = JewelryModel.Create(ModelSpec.Name, JewelryModelCategoryId.Parse(ModelSpec.CategoryId), ModelSpec.Width, ModelSpec.Length, ModelSpec.IsEngravable, ModelSpec.IsRhodiumFinish, ModelSpec.BackType, ModelSpec.ClaspType, ModelSpec.ChainType);
            await _jewelryModelRepository.Create(newModel, token);

            var flagSizeMetal = await _sender.Send(new CreateSizeMetalCommand(newModel.Id, MetalSizeSpecs));
            if (flagSizeMetal.IsFailed) return Result.Fail(flagSizeMetal.Errors);

            foreach(var mainDiamondSpec in MainDiamondSpecs)
            {
                var flagMainDiamond = await _sender.Send(new CreateMainDiamondCommand(newModel.Id, mainDiamondSpec));
                if (flagMainDiamond.IsFailed) return Result.Fail(flagMainDiamond.Errors);
            }

            foreach (var sideDiamondSpec in SideDiamondSpecs)
            {

                var flagSideDiamond = await _sender.Send(new CreateSideDiamondCommand(newModel.Id, sideDiamondSpec));
                if (flagSideDiamond.IsFailed) return Result.Fail(flagSideDiamond.Errors);
            }

            await _unitOfWork.SaveChangesAsync();
            return newModel;
        }
    }
}
