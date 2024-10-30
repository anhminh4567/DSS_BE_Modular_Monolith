using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Files.Commands.AddMainDiamondImages
{
    public record MainDiamondImagesRequest(IFormFile imageFile);
    public record AddModelMainDiamondImagesCommand(string? jewelryModelId , MainDiamondImagesRequest[]? MainDiamondImagesRequests) : IRequest<Result<string[]>>;
    internal class AddModelMainDiamondImagesCommandHandler : IRequestHandler<AddModelMainDiamondImagesCommand, Result<string[]>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryModelFileService _jewelryModelFileService;
        private readonly IJewelryModelRepository _jewelryModelRepository;

        public AddModelMainDiamondImagesCommandHandler(IUnitOfWork unitOfWork, IJewelryModelFileService jewelryModelFileService, IJewelryModelRepository jewelryModelRepository)
        {
            _unitOfWork = unitOfWork;
            _jewelryModelFileService = jewelryModelFileService;
            _jewelryModelRepository = jewelryModelRepository;
        }

        public async Task<Result<string[]>> Handle(AddModelMainDiamondImagesCommand request, CancellationToken cancellationToken)
        {
            var parsedId = JewelryModelId.Parse(request.jewelryModelId);
            var getJewelry = await _jewelryModelRepository.GetById(parsedId);
            if (getJewelry is null)
                return Result.Fail(new NotFoundError("jewelry model not found"));

            if (request.MainDiamondImagesRequests is null || request.MainDiamondImagesRequests.Count() == 0)
                return Result.Fail(new NotFoundError("no side diamond images found"));

            List<BaseMainDiamondFileData> jewelryMainDiamondDatas = new();
            foreach (var image in request.MainDiamondImagesRequests)
            {
                var tobeAddedFile = new BaseMainDiamondFileData(getJewelry.MainDiamonds,new Commons.Models.FileData(image.imageFile.FileName,null,image.imageFile.ContentType,image.imageFile.OpenReadStream()));
                jewelryMainDiamondDatas.Add(tobeAddedFile);
            }
            var result = await _jewelryModelFileService.UploadBaseMainDiamond(getJewelry, jewelryMainDiamondDatas.ToArray(), cancellationToken);
            return result;

        }
    }
}
