using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Dtos.Responses;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using FluentResults;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.BankAccounts.Commands
{
    public record UpdateBankAccountQrCommand(IFormFile newQrImage) : IRequest<Result<MediaDto>>;
    internal class UpdateBankAccountQrCommandHandler : IRequestHandler<UpdateBankAccountQrCommand, Result<MediaDto>>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IMapper _mapper;
        private readonly IBlobFileServices _blobFileServices;
        private readonly IValidator<ShopBankAccountRules> _validator;
        private readonly IApplicationSettingService _applicationSettingService;

        public UpdateBankAccountQrCommandHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IMapper mapper, IBlobFileServices blobFileServices, IValidator<ShopBankAccountRules> validator, IApplicationSettingService applicationSettingService)
        {
            _optionsMonitor = optionsMonitor;
            _mapper = mapper;
            _blobFileServices = blobFileServices;
            _validator = validator;
            _applicationSettingService = applicationSettingService;
        }

        public async Task<Result<MediaDto>> Handle(UpdateBankAccountQrCommand request, CancellationToken cancellationToken)
        {
            var bankRules = _optionsMonitor.CurrentValue.ShopBankAccountRules;
            var clonedBankRule = JsonConvert.DeserializeObject<ShopBankAccountRules>(JsonConvert.SerializeObject(bankRules));
            if (clonedBankRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            var fileName = request.newQrImage.FileName.Split(".")[0];
            var fileExtension = Path.GetExtension(request.newQrImage.FileName).Replace(".", "");
            var newImage = new FileData(fileName, fileExtension, request.newQrImage.ContentType, request.newQrImage.OpenReadStream());

            var getFolders = await _blobFileServices.GetFolders(ShopBankAccountRules.BANK_QR_FOLDERS);
            if (getFolders != null)
            {
                if (getFolders.Count > 0)
                {
                    foreach (var folder in getFolders)
                    {
                        await _blobFileServices.DeleteFileAsync(folder.MediaPath);
                    }
                }
            }
            var filename = newImage.FileName + "_" + Utilities.GenerateRandomString(6)+"." + newImage.FileExtension;
            var filePath = $"{ShopBankAccountRules.BANK_QR_FOLDERS}/{filename}";
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
            clonedBankRule.BankQr = media;
            _applicationSettingService.Set(ShopBankAccountRules.Key, clonedBankRule);
            var mappedMedia = _mapper.Map<MediaDto>(media);
            return Result.Ok(mappedMedia);
            throw new NotImplementedException();
        }
    }

}
