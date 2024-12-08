namespace DiamondShop.Application.Dtos.Responses.Promotions
{
    public class PromotionUsageDetailResponseDto
    { 
        //public string PromotionUsageLimit { get; set; }
        //public string PromotionUsageLimitUnit { get; set; }
        //public string PromotionUsageRemaining { get; set; }
        //public string PromotionUsageRemainingUnit { get; set; }
        //public PromotionDto? Promotion { get; set; }
        public int? TotalCurrentUsage { get; set; }
        public decimal? TotalMoneyAmountUsage { get; set; }
        public List<string> OrderIdsUsage { get; set; } = new();
    }
}
