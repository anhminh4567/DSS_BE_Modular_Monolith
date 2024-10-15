using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Locations.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class GoogleDistance
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }

    public class GoogleDuration
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }

    public class GoogleElement
    {
        public GoogleDistance Distance { get; set; }
        public GoogleDuration Duration { get; set; }
        public string Status { get; set; }
    }

    public class GoogleRow
    {
        public List<GoogleElement> Elements { get; set; }
    }

    public class GoogleDistanceMatrixResponse
    {
        [JsonPropertyName("destination_addresses")]
        public List<string> DestinationAddresses { get; set; }
        [JsonPropertyName("origin_addresses")]
        public List<string> OriginAddresses { get; set; }
        public List<GoogleRow> Rows { get; set; }
        public string Status { get; set; }
    }

}
