using DiamondShop.Domain.Common.Addresses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Locations.Models
{
    public class OpenApiProvince
    {
        public string Name { get; set; }
        public int Code { get; set; }
        [JsonPropertyName("division_type")]
        public string DivisionType { get; set; }
        public string Codename { get; set; }
        [JsonPropertyName("phone_code")]
        public int PhoneCode { get; set; }
        public List<OpenApiDistrict> Districts { get; set; }
    }
}
