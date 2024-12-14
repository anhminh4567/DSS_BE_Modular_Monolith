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
        public static string key = "OrderRuleVer5";
        public int ExpectedDeliveryDate { get; set; } = 7 * 3;
        public int ExpiredOrderHour { get; set; } = 24;
        public decimal MaxOrderAmountForDelivery { get; set; } = 200_000_000m;
        public decimal MaxOrderAmountForFullPayment { get; set; } = 50_000_000m;
        public int DaysWaitForCustomerToPay { get; set; } = 5;
        // only 5 order be process for customer at a time, 
        // and order is in process are, != success, != rejected , != canceled
        public int MaxOrderAmountForCustomerToPlace { get; set; } = 5;
        public int MaxRedelivery = 3;

    }
}
