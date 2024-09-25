using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Zalopays.Models
{
    public class ZalopayCallbackRequest
    {
        public string data { get; set; }
        public string mac { get; set; }
        public int type { get; set; }
    }
}
