
namespace DiamondShop.Domain.BusinessRules
{
    public class CartModelRules
    {
        public const string key = "CartModelRulesVer.1";
        public static CartModelRules Default = new CartModelRules();
        public static string Type = typeof(CartModelRules).AssemblyQualifiedName;
        public int MaxItemPerCart { get; set; } = 10;
    }
}
