using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Files.Commands.AddCategoryImages
{
    public record CategoryImagesRequest(string? sideDiamondOptId , string metalId, IFormFile imageFile);
    public record AddModelCategoryImagesCommand(string jewelryModelId, List<CategoryImagesRequest> categoryImages) : IRequest<Result<string[]>>;
    internal class AddModelCategoryImagesCommandHandler : IRequestHandler<AddModelCategoryImagesCommand, Result<string[]>>
    {
        private readonly IJewelryModelFileService _jewelryModelFileService;
        private readonly IJewelryModelService _jewelryModelService;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;

        public AddModelCategoryImagesCommandHandler(IJewelryModelFileService jewelryModelFileService, IJewelryModelService jewelryModelService, IJewelryModelRepository jewelryModelRepository, ISideDiamondRepository sideDiamondRepository)
        {
            _jewelryModelFileService = jewelryModelFileService;
            _jewelryModelService = jewelryModelService;
            _jewelryModelRepository = jewelryModelRepository;
            _sideDiamondRepository = sideDiamondRepository;
        }

        public async Task<Result<string[]>> Handle(AddModelCategoryImagesCommand request, CancellationToken cancellationToken)
        {
            var parsedJewelryModelId = JewelryModelId.Parse(request.jewelryModelId);
            var getJewelry = await _jewelryModelRepository.GetById(parsedJewelryModelId);
            if (getJewelry is null)
                return Result.Fail(new NotFoundError("jewelry model not found"));

            if (request.categoryImages is null || request.categoryImages.Count() == 0)
                return Result.Fail(new NotFoundError("no side diamond images found"));
            // get all side diamonds
            
            var parsedJewelrySideDiamondId = request.categoryImages
                    .Select(x => SideDiamondOptId.Parse(x.sideDiamondOptId)).ToList();
            var getSideDiamondOptions = await _sideDiamondRepository.GetSideDiamondOption(parsedJewelrySideDiamondId);
            var getJewelryMetals = getJewelry.SizeMetals.DistinctBy(x => x.MetalId).Select(x => x.MetalId).ToList();
            if (getSideDiamondOptions!.Count == 0)
                return Result.Fail(new NotFoundError("side diamond options not found"));
            foreach (var image in request.categoryImages)
            {
                if (FileUltilities.IsImageFileContentType(image.imageFile.ContentType) == false)
                    return Result.Fail(new ConflictError("Contain Invalid file type, has to be image"));
                if (image.sideDiamondOptId == null)
                    return Result.Fail(new ConflictError($"this image at position {request.categoryImages.IndexOf(image)} does not contain option id? :"));
                if(image.metalId == null ||  getJewelryMetals.Contains(MetalId.Parse(image.metalId)) == false)
                    return Result.Fail(new ConflictError($"this image at position {request.categoryImages.IndexOf(image)} does not contain metal id? :"));
            }

            var mappedListFromRequest = request.categoryImages.Select(x =>
            {
                MetalId metalId = MetalId.Parse(x.metalId);
                SideDiamondOpt? sideDiamondOpt = null;
                if (x.sideDiamondOptId != null)
                    sideDiamondOpt = getSideDiamondOptions.FirstOrDefault(k => k.Id == SideDiamondOptId.Parse(x.sideDiamondOptId));
                return new CategoryFileData(metalId, sideDiamondOpt, new FileData(x.imageFile.FileName, null, x.imageFile.ContentType, x.imageFile.OpenReadStream()));
            }).ToList();

            var result = await _jewelryModelFileService.UploadCategory(getJewelry, mappedListFromRequest.ToArray(), cancellationToken);
            return result;
        }
    }
}
