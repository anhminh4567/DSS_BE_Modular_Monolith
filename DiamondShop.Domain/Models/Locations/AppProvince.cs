using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Locations
{
    public class AppProvince
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ApiId { get; set; }
        public bool IsActive { get; set; }
        public string[]? SupportedNames { get; set; }
    }
}
