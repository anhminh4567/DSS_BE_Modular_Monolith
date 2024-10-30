using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Files.Commands.AddSideDiamondImages
{
    public record SideDiamondImagesRequest(string[]? sideDiamondOptionIds, IFormFile images);
    public record AddModelSideDiamondImagesCommand(string jewelryModelId, SideDiamondImagesRequest[]? SideDiamondImagesRequests) : IRequest<Result<string[]>>;
    internal class AddModelSideDiamondImagesCommandHandler : IRequestHandler<AddModelSideDiamondImagesCommand, Result<string[]>>
    {
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryModelFileService _jewelryModelFileService;
        private readonly ISideDiamondRepository _sideDiamondRepository;

        public AddModelSideDiamondImagesCommandHandler(IJewelryModelRepository jewelryModelRepository, IUnitOfWork unitOfWork, IJewelryModelFileService jewelryModelFileService, ISideDiamondRepository sideDiamondRepository)
        {
            _jewelryModelRepository = jewelryModelRepository;
            _unitOfWork = unitOfWork;
            _jewelryModelFileService = jewelryModelFileService;
            _sideDiamondRepository = sideDiamondRepository;
        }

        public async Task<Result<string[]>> Handle(AddModelSideDiamondImagesCommand request, CancellationToken cancellationToken)
        {
            var parsedId = JewelryModelId.Parse(request.jewelryModelId);
            var getJewelry = await _jewelryModelRepository.GetById(parsedId);
            if (getJewelry is null)
                return Result.Fail(new NotFoundError("jewelry model not found"));

            if(request.SideDiamondImagesRequests is null || request.SideDiamondImagesRequests.Count() == 0)
                return Result.Fail(new NotFoundError("no side diamond images found"));
            // get all side diamonds
            var parsedJewelrySideDiamondId = request.SideDiamondImagesRequests
                .Where(x => x.sideDiamondOptionIds != null)
                .SelectMany(x => x.sideDiamondOptionIds
                    .Select(x => SideDiamondOptId.Parse(x)))
                .ToList();
            var getSideDiamondOptions = await _sideDiamondRepository.GetSideDiamondOption(parsedJewelrySideDiamondId);
            if(getSideDiamondOptions!.Count == 0)
                return Result.Fail(new NotFoundError("side diamond options not found"));
            List<BaseSideDiamondFileData> jewelrySideDiamondDatas = new();
            foreach (var image in request.SideDiamondImagesRequests)
            {
                if (FileUltilities.IsImageFileContentType(image.images.ContentType) == false)
                    return Result.Fail(new ConflictError("Contain Invalid file type, has to be image"));
                if (image.sideDiamondOptionIds == null)
                    continue; // dont add if no side diamnds given
                var parsedIds = image.sideDiamondOptionIds.Select(x => SideDiamondOptId.Parse(x)).ToList();
                var getSideDiamonds = getSideDiamondOptions.Where(x => parsedIds.Contains(x.Id)).ToList();
                // if dont found enought options so that it match the jewelry schema, then something wrong with this, skip
                if (getSideDiamonds.Count != getJewelry.SideDiamonds.Count)
                    continue; // dont add if not all side diamonds are given or not correct to model schema
                var tobeAddedFile = new BaseSideDiamondFileData(getSideDiamonds,new FileData(image.images.FileName,null, image.images.ContentType, image.images.OpenReadStream()));
                jewelrySideDiamondDatas.Add(tobeAddedFile);
            }
            var result = await _jewelryModelFileService.UploadBaseSideDiamond(getJewelry, jewelrySideDiamondDatas.ToArray(), cancellationToken);
            return result;

        }
    }
}
