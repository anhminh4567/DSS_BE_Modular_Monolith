using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Files.Commands.AddMetalImages
{
    public record AddModelMetalImagesCommand(string jewelryId, string metalId, IFormFile[] images) : IRequest<Result<string[]>>;
    internal class AddModelMetalImagesCommandHandler : IRequestHandler<AddModelMetalImagesCommand, Result<string[]>>
    {
        private readonly IJewelryModelFileService _jewelryModelFileService;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddModelMetalImagesCommandHandler(IJewelryModelFileService jewelryModelFileService, IJewelryModelRepository jewelryModelRepository, IUnitOfWork unitOfWork)
        {
            _jewelryModelFileService = jewelryModelFileService;
            _jewelryModelRepository = jewelryModelRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string[]>> Handle(AddModelMetalImagesCommand request, CancellationToken cancellationToken)
        {
            var parsedId = JewelryModelId.Parse(request.jewelryId);
            var getJewelry = await _jewelryModelRepository.GetById(parsedId);
            if (getJewelry is null)
                return Result.Fail(new NotFoundError("jewelry model not found"));
            
            var parsedMetalId = MetalId.Parse(request.metalId);
            if (getJewelry.SizeMetals.Select(x => x.MetalId).Contains(parsedMetalId) is false)
                return Result.Fail(new NotFoundError("metal not found in this jewelry"));

            if (request.images.Any(x => FileUltilities.IsImageFileContentType(x.ContentType) == false))
                return Result.Fail(new ConflictError("Contain Invalid file type, has to be image"));
            BaseMetalFileData[] jewelryMetalDatas = request.images.Select(x => new BaseMetalFileData(parsedMetalId , new FileData(x.FileName, null, x.ContentType, x.OpenReadStream()))).ToArray();
            var uploadedResult = await _jewelryModelFileService.UploadBaseMetal(getJewelry, jewelryMetalDatas, cancellationToken);
            return uploadedResult;
        }
    }
}
