namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals
{
    public class PaypalPurchaseUnitItem
    {
        public string? reference_id { get; set; }
        public PaypalAmount amount { get; set; }
    }
    public class PaypalAmount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }
    public static class PaypalIntent
    {
        public const string CAPTURE = "CAPTURE";
        public const string AUTHORIZE = "AUTHORIZE";
    }
}
