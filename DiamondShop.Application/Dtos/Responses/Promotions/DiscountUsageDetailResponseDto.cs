namespace DiamondShop.Application.Dtos.Responses.Promotions
{
    public class DiscountUsageDetailResponseDto
    {
        //public DiscountDto? Discount { get; set; }
        public int? TotalUsageFromOrders { get; set; }
        public decimal? TotalDiscountAmountFromOrder { get; set; }
        public List<string> OrderIdsUsage { get; set; } = new();
    }
}
