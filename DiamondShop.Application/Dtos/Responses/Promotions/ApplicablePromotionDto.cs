using DiamondShop.Domain.Models.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Promotions
{
    public class ApplicablePromotionDto
    {
        public int TotalPromotionsCount { get => Promotions.Count; }
        public int ApplicablePromotionsCount { get => Promotions.Where(x => x.IsApplicable).Count(); }
        public List<PromoResponse> Promotions { get; set; } = new();
    }
    public class PromoResponse
    {
        public PromoResponse(decimal amountSaved, string promoId, PromotionDto promotionDto, bool isApplicable)
        {
            AmountSaved = amountSaved;
            PromoId = promoId;
            PromotionDto = promotionDto;
            IsApplicable = isApplicable;
        }

        public decimal AmountSaved { get; set; }
        public string PromoId { get; set; }
        public PromotionDto PromotionDto { get; set; }
        public bool IsApplicable { get; set; }
    }
}
