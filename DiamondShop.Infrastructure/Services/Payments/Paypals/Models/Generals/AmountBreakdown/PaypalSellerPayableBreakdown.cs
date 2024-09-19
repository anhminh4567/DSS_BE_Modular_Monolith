namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals.AmountBreakdown
{
    public class PaypalSellerPayableBreakdown
    {
        public PaypalMoney gross_amount { get; set; }
        public PaypalMoney paypal_fee { get; set; }
        public PaypalMoney net_amount { get; set; }
        public PaypalMoney total_refunded_amount { get; set; }
    }
}
