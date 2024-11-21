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
        public int ExpectedDeliveryDate = 7 * 3;
        public int ExpiredOrderHour = 24;
    }
}
