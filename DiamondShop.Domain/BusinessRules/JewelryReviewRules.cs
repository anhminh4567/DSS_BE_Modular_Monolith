namespace DiamondShop.Domain.BusinessRules
{
    public class JewelryReviewRules
    {
        public static JewelryReviewRules Default = new JewelryReviewRules();
        public static string Type = typeof(JewelryReviewRules).AssemblyQualifiedName;
        public static string key = "JewelryReviewRule";
        public int MaxContentSizeInMb = 15;
        public int MaxFileAllowed = 5;
        public int MaxContentSize { get => MaxContentSizeInMb * 1024 * 1024; }
        public List<string> AllowedContentType = new List<string>()
        {
            "image/png",
            "image/jpeg",
            "image/jpg",
            "image/gif",
            "video/mp4",
        };
    }
}
