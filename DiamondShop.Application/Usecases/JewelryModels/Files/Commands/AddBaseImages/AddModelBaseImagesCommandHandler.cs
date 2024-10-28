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

namespace DiamondShop.Application.Usecases.JewelryModels.Files.Commands.AddBaseImages
{
    public record AddModelBaseImagesCommand(string jewelryModelId, IFormFile[] images) : IRequest<Result<string[]>>;
    internal class AddModelBaseImagesCommandHandler : IRequestHandler<AddModelBaseImagesCommand, Result<string[]>>
    {
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryModelFileService _jewelryModelFileService;

        public AddModelBaseImagesCommandHandler(IJewelryModelRepository jewelryModelRepository, IUnitOfWork unitOfWork, IJewelryModelFileService jewelryModelFileService)
        {
            _jewelryModelRepository = jewelryModelRepository;
            _unitOfWork = unitOfWork;
            _jewelryModelFileService = jewelryModelFileService;
        }

        public async Task<Result<string[]>> Handle(AddModelBaseImagesCommand request, CancellationToken cancellationToken)
        {
            var parsedId = JewelryModelId.Parse(request.jewelryModelId);
            var getJewelry = await _jewelryModelRepository.GetById(parsedId);
            if (getJewelry is null)
                return Result.Fail(new NotFoundError("jewelry not found"));
            
            if (request.images.Any(x => FileUltilities.IsImageFileContentType(x.ContentType) == false))
                return Result.Fail(new ConflictError("Contain Invalid file type, has to be image"));
            FileData[] diamondFileDatas = request.images.Select(x => new FileData(x.FileName, null, x.ContentType, x.OpenReadStream())).ToArray();
            var uploadedResult = await _jewelryModelFileService.UploadBase(getJewelry, diamondFileDatas, cancellationToken);
            return uploadedResult;
        }
    }
}
