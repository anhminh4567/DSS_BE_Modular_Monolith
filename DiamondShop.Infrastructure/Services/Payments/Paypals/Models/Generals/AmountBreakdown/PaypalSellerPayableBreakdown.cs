using Microsoft.IdentityModel.Tokens;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals.AmountBreakdown
{
    public class PaypalSellerPayableBreakdown
    {
        // gross amout = the amount you intent to give back
        public PaypalMoney gross_amount { get; set; }
        public PaypalMoney paypal_fee { get; set; }
        // net amount is the real money the user receive. it equal to the gross amount - minus paypal fee
        public PaypalMoney net_amount { get; set; }
        // the total ACCORDING to the original like you give 100, you refund 20, and now you refund 30
        // then the gross is 30, the total is 50
        public PaypalMoney total_refunded_amount { get; set; }
    }
}
