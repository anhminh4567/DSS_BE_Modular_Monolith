using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options
{
    public class JwtOptions
    {
        public const string JwtSection = "Jwt";
        public string Audience { get; init; } = string.Empty;
        public string MetadataUrl { get; init; } = string.Empty;
        public bool RequireHttpsMetadata { get; init; }
        public string ValidIssuer { get; set; } = string.Empty;
        public string SigningKey { get; set; } = string.Empty;
    }
}
