namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals.AmountBreakdown
{
    public class PaypalSellerReceivableBreakdown
    {
        public PaypalMoney gross_amount { get; set; }
        public PaypalMoney paypal_fee { get; set; }
        public PaypalMoney net_amount { get; set; }
        public PaypalMoney receivable_amount { get; set; }
        public PaypalMoney paypal_fee_in_receivable_currency{ get; set; }
        public PaypalExchangeRate exchange_rate { get; set; }
    }
}
