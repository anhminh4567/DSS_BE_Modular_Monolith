using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Locations
{
    public class AppCities
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }
    public class AppDistrict
    {
        public int Id { get; set; }
        public int ProvinceId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }
    public class AppWard
    {
        public int Id { get; set; }
        public int DistrictId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }
}
