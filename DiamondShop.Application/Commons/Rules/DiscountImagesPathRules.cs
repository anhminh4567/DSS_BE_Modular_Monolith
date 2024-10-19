namespace DiamondShop.Application.Commons.Rules
{
    internal static class DiscountImagesPathRules
    {
        internal const string PARENT_FOLDER = "Discounts";
        internal const string DELIMITER = "/";
        internal static string GetBasePath()
        {
            return $"{PARENT_FOLDER}/";
        }
    }
}
