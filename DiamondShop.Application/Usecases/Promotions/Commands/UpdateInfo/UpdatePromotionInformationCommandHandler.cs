using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Rules;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Dtos.Requests.Promotions;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.ErrorMessages;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Commands.UpdateInfo
{

    public record UpdatePromotionInformationCommand(string? promotionId,bool? isExcludeQualifierProduct, string? name, string? description, string? PromoCode, UpdateStartEndDate? UpdateStartEndDate) : IRequest<Result<Promotion>>;
    internal class UpdatePromotionInformationCommandHandler : IRequestHandler<UpdatePromotionInformationCommand, Result<Promotion>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPromotionServices _promotionServices;
        private readonly IBlobFileServices _blobFileServices;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public UpdatePromotionInformationCommandHandler(IUnitOfWork unitOfWork, IPromotionRepository promotionRepository, IPromotionServices promotionServices, IBlobFileServices blobFileServices, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _unitOfWork = unitOfWork;
            _promotionRepository = promotionRepository;
            _promotionServices = promotionServices;
            _blobFileServices = blobFileServices;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<Promotion>> Handle(UpdatePromotionInformationCommand request, CancellationToken cancellationToken)
        {
            var parsedId = PromotionId.Parse(request.promotionId!);
            var getPromotion = await _promotionRepository.GetById(parsedId);
            var promoRule = _optionsMonitor.CurrentValue.PromotionRule;
            if (getPromotion is null)
                return Result.Fail(PromotionError.NotFound);

            if (request.name != null)
                getPromotion.Name = request.name;

            if (request.description != null)
                getPromotion.Description = request.description;
            if(request.isExcludeQualifierProduct != null)
                getPromotion.IsExcludeQualifierProduct = request.isExcludeQualifierProduct.Value;
            if(request.PromoCode != null)
            {
                if (request.PromoCode.Length < promoRule.MinCode || request.PromoCode.Length > promoRule.MaxCode)
                    return Result.Fail($"Mã khuyến mãi mới không hợp lệ, có chiều dài phải lớn hơn {promoRule.MinCode} và bé hơn {promoRule.MaxCode}");
                getPromotion.PromoCode = request.PromoCode;
            }
            if (request.UpdateStartEndDate != null)
            {
                var parsedStartResults = DateTime.TryParseExact(request.UpdateStartEndDate.startDate, DateTimeFormatingRules.DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime startParsed);
                var parsedEndResults = DateTime.TryParseExact(request.UpdateStartEndDate.endDate, DateTimeFormatingRules.DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime endParsed);
                if(parsedStartResults == true && parsedEndResults == true)
                    getPromotion.ChangeActiveDate(startParsed, endParsed);
               
                else if(parsedStartResults == true && parsedEndResults == false)
                    getPromotion.ChangeStartDate(startParsed);
                
                else if(parsedEndResults == true && parsedStartResults == false)
                    getPromotion.ChangeEndDate(endParsed);
                //if (request.UpdateStartEndDate.startDate != null && request.UpdateStartEndDate.endDate != null)
                //{
                //    DateTime startParsed = DateTime.ParseExact(request.UpdateStartEndDate.startDate, DateTimeFormatingRules.DateTimeFormat, null);
                //    DateTime endParsed = DateTime.ParseExact(request.UpdateStartEndDate.endDate, DateTimeFormatingRules.DateTimeFormat, null);
                //    getPromotion.ChangeActiveDate(startParsed, endParsed);
                //}
                
            }
            await _promotionRepository.Update(getPromotion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return getPromotion;
        }
    }
}
