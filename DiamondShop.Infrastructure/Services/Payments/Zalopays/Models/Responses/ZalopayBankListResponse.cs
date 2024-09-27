namespace DiamondShop.Infrastructure.Services.Payments.Zalopays.Models.Responses
{

    public class ZalopayBankListResponse
    {
        public string returncode { get; set; }
        public string returnmessage { get; set; }
        public Dictionary<string, List<ZalopayBankResponse>> banks { get; set; }
    }

}

