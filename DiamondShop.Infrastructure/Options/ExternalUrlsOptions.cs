using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options
{
    public class ExternalUrlsOptions
    {
        public const string Section = "ExternalUrls";
        public AzureConnection Azure { get; set; }
    }
    public class AzureConnection
    {
        public string ConnectionString { get; set; }
        public string BaseUrl { get; set; }
        public string ContainerName { get; set; }
    }

}
