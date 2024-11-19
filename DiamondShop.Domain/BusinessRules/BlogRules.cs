namespace DiamondShop.Domain.BusinessRules
{
    public class BlogRules
    {
        public static BlogRules Default = new BlogRules();
        public static string Type = typeof(BlogRules).AssemblyQualifiedName;
        public static string key = "BlogRule";
        public int MaxContentSizeInMb = 15;
        public int MaxContentSize { get => MaxContentSizeInMb * 1024 * 1024; }
        public List<string> AllowedThumbnailType = new List<string>()
        {
            "image/png",
            "image/jpeg",
            "image/jpg",
        };
        public string MaxContentSizeError { get => $"Thumbnail vượt kích thước {MaxContentSizeInMb} Mb"; }
        public string ContentTypeError { get => $"Thumbnail không được hỗ trợ"; }

    }
}
