using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Frontends.Commands
{
    public record SetNewImageCommand(IFormFile imageFile) : IRequest<Result<Media>>;
    internal class SetNewImageCommandHandler : IRequestHandler<SetNewImageCommand, Result<Media>>
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IValidator<FrontendDisplayConfiguration> _validator;
        private readonly IBlobFileServices _blobFileServices;

        public SetNewImageCommandHandler(IApplicationSettingService applicationSettingService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IValidator<FrontendDisplayConfiguration> validator, IBlobFileServices blobFileServices)
        {
            _applicationSettingService = applicationSettingService;
            _optionsMonitor = optionsMonitor;
            _validator = validator;
            _blobFileServices = blobFileServices;
        }

        public async Task<Result<Media>> Handle(SetNewImageCommand request, CancellationToken cancellationToken)
        {
            var fileName = request.imageFile.FileName.Split(".")[0];  
            var fileExtension = Path.GetExtension(request.imageFile.FileName).Replace(".","");
            var newImage = new FileData(fileName,fileExtension, request.imageFile.ContentType,request.imageFile.OpenReadStream());

            var isValid = IsValidToUpload().Result;
            if (isValid.IsFailed)
            {
                return Result.Fail(isValid.Errors);
            }
            var filename = newImage.FileName + "_" + Utilities.GenerateRandomString(6) + newImage.FileExtension;
            var filePath = $"{FrontendDisplayConfiguration.CAROUSEL_FOLDERS}/{filename}";
            var result = await _blobFileServices.UploadFileAsync(filePath, newImage.Stream, newImage.contentType);
            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }
            var media = new Media()
            {
                ContentType = newImage.contentType,
                MediaName = filename,
                MediaPath = result.Value,
            };
            return Result.Ok(media);
            throw new NotImplementedException();
        }
        private async Task<Result> IsValidToUpload()
        {
            var frontendRule = _optionsMonitor.CurrentValue.FrontendDisplayConfiguration;
            var getFolders = await _blobFileServices.GetFolders(FrontendDisplayConfiguration.CAROUSEL_FOLDERS);
            if (getFolders != null)
            {
                if (getFolders.Count >= frontendRule.MaxCarouselImages)
                {
                    return Result.Fail("số lượng set tối đa cho hình carousel là " + frontendRule.MaxCarouselImages);
                }
                return Result.Ok();
            }
            throw new Exception("không lấy được file , lỗi nghiêm trọng");
        }
       
    }
}
