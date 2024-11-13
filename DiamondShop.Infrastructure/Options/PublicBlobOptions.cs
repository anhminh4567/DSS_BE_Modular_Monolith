using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options
{
    public class PublicBlobOptions
    {
        public static string Section = "PublicBlob";
        public string ContainerName { get; set; }
        public string ShopIcon { get; set; }
        public string DiamondRingIcon { get; set; }
        public string DiamondIcon { get; set; }
        public string GetPath(ExternalUrlsOptions externalUrlsOptions,string fileName)
        {
            return $"{externalUrlsOptions.Azure.BaseUrl}/{ContainerName}/{fileName}";
        }
    }
}
