namespace DiamondShop.Domain.BusinessRules
{
    public static class OrderRules
    {
        public static int ExpectedDeliveryDate = 7 * 3;
        public static int ExpiredOrderHour = 24;
        public static int OrderCodeLength = 6;
    }
    public class OrderRule
    {
        public static OrderRule Default = new OrderRule();
        public static string Type = typeof(OrderRule).AssemblyQualifiedName;
        public static string key = "OrderRuleVer2";
        public int ExpectedDeliveryDate { get; set; } = 7 * 3;
        public int ExpiredOrderHour { get; set; } = 24;
        public decimal MaxOrderAmountForDelivery { get; set; } = 100_000_000m;
        public decimal MaxOrderAmountForFullPayment { get; set; } = 50_000_000m;
        public int DaysWaitForCustomerToPay { get; set; } = 5;

    }
}
