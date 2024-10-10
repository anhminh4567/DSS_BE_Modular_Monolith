using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.MainDiamonds.Commands.Create;
using DiamondShop.Application.Usecases.SideDiamonds.Commands;
using DiamondShop.Application.Usecases.SizeMetals.Commands.Create;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryModels.Commands.Create
{
    public record CreateJewelryModelCommand(JewelryModelRequestDto ModelSpec, List<MainDiamondRequestDto>? MainDiamondSpecs, List<SideDiamondRequestDto>? SideDiamondSpecs, List<ModelMetalSizeRequestDto> MetalSizeSpecs) : IRequest<Result<JewelryModel>>;
    internal class CreateJewelryModelCommandHandler : IRequestHandler<CreateJewelryModelCommand, Result<JewelryModel>>
    {
        private readonly ISender _sender;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IJewelryModelCategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateJewelryModelCommandHandler(
            ISender sender,
            IJewelryModelCategoryRepository categoryRepository,
            IJewelryModelRepository jewelryModelRepository,
        IUnitOfWork unitOfWork)
        {
            _sender = sender;
            _jewelryModelRepository = jewelryModelRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<JewelryModel>> Handle(CreateJewelryModelCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out JewelryModelRequestDto modelSpec, out List<MainDiamondRequestDto>? mainDiamondSpecs,
                out List<SideDiamondRequestDto>? sideDiamondSpecs, out List<ModelMetalSizeRequestDto> metalSizeSpecs);
            var category = _categoryRepository.GetQuery().FirstOrDefault(p => p.Id == JewelryModelCategoryId.Parse(modelSpec.CategoryId));
            if(category is null) return Result.Fail(new NotFoundError("Can't find model category object."));
            var newModel = JewelryModel.Create(modelSpec.Name, category.Id, modelSpec.Width, modelSpec.Length, modelSpec.IsEngravable, modelSpec.IsRhodiumFinish, modelSpec.BackType, modelSpec.ClaspType, modelSpec.ChainType);
            await _jewelryModelRepository.Create(newModel, token);
            
            var flagSizeMetal = await _sender.Send(new CreateSizeMetalCommand(newModel.Id, metalSizeSpecs));
            if (flagSizeMetal.IsFailed) return Result.Fail(flagSizeMetal.Errors);

            if (mainDiamondSpecs != null)
            {

                foreach (var mainDiamondSpec in mainDiamondSpecs)
                {
                    var flagMainDiamond = await _sender.Send(new CreateMainDiamondCommand(newModel.Id, mainDiamondSpec));
                    if (flagMainDiamond.IsFailed) return Result.Fail(flagMainDiamond.Errors);
                    newModel.MainDiamonds.Add(flagMainDiamond.Value);
                }
            }
            if (sideDiamondSpecs != null)
            {
                foreach (var sideDiamondSpec in sideDiamondSpecs)
                {

                    var flagSideDiamond = await _sender.Send(new CreateSideDiamondCommand(newModel.Id, sideDiamondSpec));
                    if (flagSideDiamond.IsFailed) return Result.Fail(flagSideDiamond.Errors);
                    newModel.SideDiamonds.Add(flagSideDiamond.Value);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            newModel.Category = category;
            return newModel;
        }
    }
}
