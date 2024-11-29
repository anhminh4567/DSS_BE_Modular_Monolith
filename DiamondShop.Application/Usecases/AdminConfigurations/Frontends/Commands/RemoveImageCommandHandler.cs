using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Frontends.Commands
{
    public record RemoveImageCommand(string absoluteUrl) : IRequest<Result>;
    internal class RemoveImageCommandHandler : IRequestHandler<RemoveImageCommand, Result>
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IValidator<FrontendDisplayConfiguration> _validator;
        private readonly IBlobFileServices _blobFileServices;

        public RemoveImageCommandHandler(IApplicationSettingService applicationSettingService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IValidator<FrontendDisplayConfiguration> validator, IBlobFileServices blobFileServices)
        {
            _applicationSettingService = applicationSettingService;
            _optionsMonitor = optionsMonitor;
            _validator = validator;
            _blobFileServices = blobFileServices;
        }

        public async Task<Result> Handle(RemoveImageCommand request, CancellationToken cancellationToken)
        {
            var absolutePath = request.absoluteUrl;
            var isValid = await IsValidToRemove();
            if (isValid.IsFailed)
            {
                return Result.Fail(isValid.Errors);
            }
            string relativePathToDelete = null;
            if (absolutePath.StartsWith("https://") == false && absolutePath.StartsWith("http://") == false)
                relativePathToDelete = _blobFileServices.ToRelativePath(absolutePath);
            else
                relativePathToDelete = absolutePath;
            await _blobFileServices.DeleteFileAsync(relativePathToDelete);
            throw new NotImplementedException();
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
