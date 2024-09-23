namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals
{
    public class PaypalPurchaseUnitItem
    {
        public string? reference_id { get; set; }
        public PaypalAmount? amount { get; set; }
        public PaypalPurchaseUnitPayment? payments { get; set; }
    }
    public class PaypalPurchaseUnitPayment
    {
        public List<PaypalPaymentsCapture> captures { get; set; }
    }
}
