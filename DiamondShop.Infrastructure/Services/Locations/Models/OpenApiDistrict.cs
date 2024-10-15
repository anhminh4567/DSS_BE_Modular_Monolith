using Azure.Core.Serialization;
using System.Text.Json.Serialization;

namespace DiamondShop.Infrastructure.Services.Locations.Models
{
    public class OpenApiDistrict
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("code")]
        public int Code { get; set; }
    }
}
