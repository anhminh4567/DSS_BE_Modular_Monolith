using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Commands.Create
{
    public record CreatePromotionCommand(string startDateTime, string endDateTime,string name, string promoCode, string description, RedemptionMode RedemptionMode,bool isExcludeQualifierProduct = true, int priority = 1  ) : IRequest<Result<Promotion>>;
    internal class CreatePromotionCommandHandler : IRequestHandler<CreatePromotionCommand, Result<Promotion>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IRequirementRepository _requirementRepository;
        private readonly IGiftRepository _giftRepository;

        public CreatePromotionCommandHandler(IUnitOfWork unitOfWork, IPromotionRepository promotionRepository, IRequirementRepository requirementRepository, IGiftRepository giftRepository)
        {
            _unitOfWork = unitOfWork;
            _promotionRepository = promotionRepository;
            _requirementRepository = requirementRepository;
            _giftRepository = giftRepository;
        }

        public async Task<Result<Promotion>> Handle(CreatePromotionCommand request, CancellationToken cancellationToken)
        {
            DateTime startParsed = DateTime.ParseExact(request.startDateTime,DateTimeFormatingRules.DateTimeFormat,null);
            DateTime endParsed = DateTime.ParseExact(request.endDateTime, DateTimeFormatingRules.DateTimeFormat, null);

            var newPromo = Promotion.Create(request.name, request.promoCode,request.description,startParsed,endParsed,request.priority,request.isExcludeQualifierProduct,request.RedemptionMode);
            var getPromoByCode = await _promotionRepository.GetByCode(request.promoCode);
            if (getPromoByCode != null)
                return Result.Fail("Đã có 1 promotion với mã code như vậy, hãy đổi mã code");
            await _promotionRepository.Create(newPromo);
            await _unitOfWork.SaveChangesAsync();
            return newPromo;
        }
    }
}
