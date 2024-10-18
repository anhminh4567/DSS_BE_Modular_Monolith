using DiamondShop.Domain.Models.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Rules
{
    internal static class PromotionImagesPathRules
    {
        internal const string PARENT_FOLDER = "Promotions";
        internal const string DELIMITER = "/";
        internal static string GetBasePath()
        {
            return $"{PARENT_FOLDER}/";
        }
        internal static string GetRandomizedFileName(string fileName)
        {
            return $"{fileName}_{DateTime.UtcNow.ToString("yyMMddHHmmss")}";
        }
    }
}
