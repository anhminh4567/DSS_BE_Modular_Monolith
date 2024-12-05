
namespace DiamondShop.Domain.BusinessRules
{
    public class CartModelRules
    {
        public const string key = "CartModelRulesVer.2";
        public static CartModelRules Default = new CartModelRules();
        public static string Type = typeof(CartModelRules).AssemblyQualifiedName;
        public int MaxItemPerCart { get; set; } = 10;
        //TODO: check xem giỏ có vượt quá giá trị này ko , quá thì qua quầy ko giao hàng

        public decimal MaxPriceAcceptableForDelivery { get; set; } = 400_000_000m;
        //TODO: check xem giỏ có vượt quá giá trị này ko , quá thì ko cho đặt luôn bắt tới shop do quá cao, cùng lắm lock hàng
        public decimal MaxPriceAcceptableForPlacement { get; set; } = 1_000_000_000m;
    }
}
