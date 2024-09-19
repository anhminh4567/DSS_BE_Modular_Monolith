namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals.AmountBreakdown
{
    public class PaypalAmountBreakdown
    {
        public string currency_code { get; set; }
        public string value { get; set; }
        public PaypalBreakdown breakdown { get; set; }
    }
}

