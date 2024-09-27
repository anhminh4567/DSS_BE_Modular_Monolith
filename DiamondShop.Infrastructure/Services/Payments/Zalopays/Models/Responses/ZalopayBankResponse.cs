namespace DiamondShop.Infrastructure.Services.Payments.Zalopays.Models.Responses
{

    public class ZalopayBankResponse
    {
        public string bankcode { get; set; }
        public string name { get; set; }
        public int displayorder { get; set; }
        public int pmcid { get; set; }
    }
}

