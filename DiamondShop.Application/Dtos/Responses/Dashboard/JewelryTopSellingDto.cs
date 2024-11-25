using DiamondShop.Domain.Models.Jewelries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Dashboard
{
    public class JewelryTopSellingDto
    {
        public string ModelName { get; set; }
        public string MetalName { get; set; }
        public decimal Revenue { get; set; }
        public JewelryTopSellingDto(string modelName, string metalName, decimal revenue)
        {
            ModelName = modelName;
            MetalName = metalName;
            Revenue = revenue;
        }

    }
}
