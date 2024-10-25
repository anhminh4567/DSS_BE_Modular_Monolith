using DiamondShop.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Locations
{
    public class AppProvince : Entity<string>
    {
        public string Name { get; set; }
        public string ApiId { get; set; }
        public bool IsActive { get; set; }
    }
}
