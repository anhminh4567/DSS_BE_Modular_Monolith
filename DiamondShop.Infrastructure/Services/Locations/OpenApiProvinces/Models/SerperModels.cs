using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Locations.Models
{
    internal class SerperModels
    {
    }
    public class OpeningHours
    {
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
        public string Monday { get; set; }
    }

    public class SerperPlace
    {
        public int Position { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal? Rating { get; set; }
        public int? RatingCount { get; set; }
        public string? Type { get; set; }
        public List<string> Types { get; set; }
        public string? Website { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Menu { get; set; }
        public OpeningHours? OpeningHours { get; set; }
        public List<string>? BookingLinks { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string Cid { get; set; }
        public string Fid { get; set; }
        public string PlaceId { get; set; }
    }

    public class SerperRootResponse
    {
        public string ll { get; set; }
        public List<SerperPlace> Places { get; set; }
        public int Credits { get; set; }
    }
}
