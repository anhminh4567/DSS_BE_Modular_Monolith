namespace DiamondShop.Infrastructure.Services.Payments.Zalopays.Models
{
    public class ZalopayItem
    {
        //this is the field of your app, not zalopay
        // Example:
        public string name { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal sale_price { get; set; }

    }
}
