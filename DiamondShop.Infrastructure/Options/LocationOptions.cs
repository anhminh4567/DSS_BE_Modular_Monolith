using DiamondShop.Domain.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options
{
    public class LocationOptions
    {
        public LocationRules ShopOrignalLocation { get; set; } = new();

    }
}
