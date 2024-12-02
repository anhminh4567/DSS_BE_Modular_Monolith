using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Pdfs.Models
{
    public class OrderInvoiceModels
    {
        public Order Order { get; set; }
        public Account Account { get; set; }
        public string? IconBase64 { get; set; }
        public string? DiamondRingIconBase64 { get; set; }
        public string? DiamondIconBase64 { get; set; }
        public string? IconPath { get; set; }
        public string? DiamondRingIconPath{ get; set; }
        public string? DiamondIconPath { get; set; }
        public string? ShopAddress { get; set; }

    }
}
