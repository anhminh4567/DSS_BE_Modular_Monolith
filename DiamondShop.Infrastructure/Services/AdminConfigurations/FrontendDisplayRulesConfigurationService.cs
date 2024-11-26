using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.FrontendDisplays;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
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

        public FrontendDisplayRulesConfigurationService(IApplicationSettingService applicationSettingService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IValidator<FrontendDisplayConfiguration> validator, IBlobFileServices blobFileServices)
        {
            _applicationSettingService = applicationSettingService;
            _optionsMonitor = optionsMonitor;
            _validator = validator;
            _blobFileServices = blobFileServices;
        }

        public Task<List<Media>> GetAllCarouselImages()
        {
            throw new NotImplementedException();
        }

        public Task<FrontendDisplayConfiguration> GetConfiguration()
        {
            return Task.FromResult(_optionsMonitor.CurrentValue.FrontendDisplayConfiguration);
        }

        public Task RemoveImage(string absolutePath)
        {
            string relativePathToDelete = null;
            if(absolutePath.StartsWith("https://") == false && absolutePath.StartsWith("http://") == false)
                relativePathToDelete = _blobFileServices.ToRelativePath(absolutePath);
            else
                relativePathToDelete = absolutePath;
            return _blobFileServices.DeleteFileAsync(relativePathToDelete);
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
                _applicationSettingService.Set(PromotionRule.key, displayRule);
            }
            return Result.Ok();
            throw new NotImplementedException();
        }

        public Task<Media> SetNewImage(FileData newImage)
        {
            throw new NotImplementedException();
        }
    }
}
