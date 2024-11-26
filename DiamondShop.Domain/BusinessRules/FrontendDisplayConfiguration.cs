namespace DiamondShop.Domain.BusinessRules
{
    public class FrontendDisplayConfiguration
    {
        public const string CAROUSEL_FOLDERS = "Carousel";
        public const string Key = "FrontendDisplayConfigurationVer.1";
        public static FrontendDisplayConfiguration Default = new FrontendDisplayConfiguration();
        public static string Type = typeof(FrontendDisplayConfiguration).AssemblyQualifiedName;
        public int MaxCarouselImages { get; set; } = 10;
        public int MinCarouselImages { get; set; } = 3;
        public int DisplayTimeInSeconds { get; set; } = 5;
    }
}
