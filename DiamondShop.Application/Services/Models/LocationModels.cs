using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Models
{
    public class LocationDistantData
    {
        public decimal Distance { get; set; }
        public string DistanceUnit { get; set; }
        public decimal TravelTime { get; set; }
        public string TravelTimeUnit { get; set; }
        public string TravelMode { get; set; }  
        public LocationDetail Origin { get; set; }
        public LocationDetail Destination { get; set; }
    }
    public class LocationDetail
    {
        public LocationDetail(string province, string district, string ward, string road)
        {
            Province = province;
            District = district;
            Ward = ward;
            Road = road;
        }

        public LocationDetail()
        {
        }

        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Road { get; set; }
    }
}
