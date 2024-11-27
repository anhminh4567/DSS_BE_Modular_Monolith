using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.FrontendDisplays;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Infrastructure.BackgroundJobs;
using DiamondShop.Infrastructure.Options;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.AdminConfigurations
{
    internal class FrontendDisplayRulesConfigurationService : IFrontendDisplayRulesConfigurationService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IValidator<FrontendDisplayConfiguration> _validator;
        private readonly IBlobFileServices _blobFileServices;
        private readonly IOptions<PublicBlobOptions> _publicBlobOptions;

        public FrontendDisplayRulesConfigurationService(IApplicationSettingService applicationSettingService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IValidator<FrontendDisplayConfiguration> validator, IBlobFileServices blobFileServices, IOptions<PublicBlobOptions> publicBlobOptions)
        {
            _applicationSettingService = applicationSettingService;
            _optionsMonitor = optionsMonitor;
            _validator = validator;
            _blobFileServices = blobFileServices;
            _publicBlobOptions = publicBlobOptions;
        }

        public Task<List<Media>> GetAllCarouselImages()
        {
            throw new NotImplementedException();
        }

        public Task<FrontendDisplayConfiguration> GetConfiguration()
        {
            return Task.FromResult(_optionsMonitor.CurrentValue.FrontendDisplayConfiguration);
        }

        public async Task<Result> RemoveImage(string absolutePath)
        {
            var isValid = await IsValidToRemove();
            if (isValid.IsFailed)
            {
                return Result.Fail(isValid.Errors);
            }
            string relativePathToDelete = null;
            if(absolutePath.StartsWith("https://") == false && absolutePath.StartsWith("http://") == false)
                relativePathToDelete = _blobFileServices.ToRelativePath(absolutePath);
            else
                relativePathToDelete = absolutePath;
            await _blobFileServices.DeleteFileAsync(relativePathToDelete);
            return Result.Ok();
        }

        public async Task<Result> SetConfiguration(FrontendDisplayConfiguration newValidatedConfiguration)
        {
            var displayRule = newValidatedConfiguration;
            var validationResult = _validator.Validate(displayRule);
            if (validationResult.IsValid is false)
            {
                Dictionary<string, object> validationErrors = new();
                validationResult.Errors
                    .ForEach(input =>
                    {
                        if (validationErrors.ContainsKey(input.PropertyName))
                        {
                            var errorList = (List<object>)validationErrors[input.PropertyName];
                            errorList.Add(input.ErrorMessage);
                        }
                        else
                            validationErrors.Add(input.PropertyName, new List<object> { input.ErrorMessage });
                    });
                ValidationError validationError = new ValidationError($"validation error ", validationErrors);
                return Result.Fail(validationError);
            }
            else
            {
                _applicationSettingService.Set(FrontendDisplayConfiguration.Key, displayRule);
            }
            return Result.Ok();
            throw new NotImplementedException();
        }

        public async Task<Result<Media>> SetNewImage(FileData newImage)
        {
            var isValid = IsValidToUpload().Result;
            if (isValid.IsFailed)
            {
                return Result.Fail(isValid.Errors);
            }
            var publicBlobOption = _publicBlobOptions.Value;
            var filename = newImage.FileName + "_" + Utilities.GenerateRandomString(6)+ newImage.FileExtension;
            var filePath = $"{FrontendDisplayConfiguration.CAROUSEL_FOLDERS}/{filename}";
            var result = await _blobFileServices.UploadFileAsync(filePath, newImage.Stream, newImage.contentType);
            if(result.IsFailed)
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
            if(getFolders != null) 
            {
                if(getFolders.Count >= frontendRule.MaxCarouselImages)
                {
                    return Result.Fail("số lượng set tối đa cho hình carousel là "+ frontendRule.MaxCarouselImages);
                }
                return Result.Ok();
            }
            throw new Exception ("không lấy được file , lỗi nghiêm trọng");
        }
        private async Task<Result> IsValidToRemove()
        {
            var frontendRule = _optionsMonitor.CurrentValue.FrontendDisplayConfiguration;
            var getFolders = await _blobFileServices.GetFolders(FrontendDisplayConfiguration.CAROUSEL_FOLDERS);
            if (getFolders != null)
            {
                if (getFolders.Count <= frontendRule.MinCarouselImages)
                {
                    return Result.Fail("số lượng set tối đa cho hình carousel là " + frontendRule.MaxCarouselImages);
                }
                return Result.Ok();
            }
            throw new Exception("không lấy được file , lỗi nghiêm trọng");
        }
    }
}
