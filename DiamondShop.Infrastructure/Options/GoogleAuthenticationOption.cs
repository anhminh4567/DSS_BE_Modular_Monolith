namespace DiamondShop.Infrastructure.Options
{
    internal class GoogleAuthenticationOption
    {
        public const string Section = "ExternalAuthenticationSection:GoogleAuthenticationOption";
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
