using DiamondShop.Domain.Models.Warranties.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses
{
    public class WarrantyDto
    {
        public string Id { get; set; }
        public WarrantyType Type { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int MonthDuration { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal Price { get; set; }
        public string PriceText
        {
            get
            {
                if (Price == 0m)
                    return "Mặc định";
                else
                    return Price.ToString("#,0.000");
            }
        }
        public string MappedName { get => $"{Name}({MonthDuration} tháng) - {PriceText}"; }
    }
}
