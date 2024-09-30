namespace DiamondShop.Infrastructure.Services.Payments.Zalopays
{

    public class ZalopayBankListResponse
    {
        public string returncode { get; set; }
        public string returnmessage { get; set; }
        public Dictionary<string, List<ZalopayBankDTO>> banks { get; set; }
    }

}

