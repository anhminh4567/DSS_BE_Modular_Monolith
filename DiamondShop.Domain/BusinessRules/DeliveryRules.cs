namespace DiamondShop.Application.Usecases.Deliveries.Commands.Create
{
    public static class DeliveryRules
    {
        // chuyen qua OrderRules
        //public static int MaxRedelivery = 3;
        public static string MaxRedeliveryError = "Đơn hàng này đã hết lượt giao hàng lại. Theo điều khoản hệ thống, đơn hàng được tính như chính người mua đã hủy";
    }
}
