using DiamondShop.Domain.Models.Warranties.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds
{
    public class DiamondWarrantyDto
    {
        public WarrantyStatus Status { get; set; }
        public string CreatedDate { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiredDate { get; set; }
        public string WarrantyType { get; set; }
        public string WarrantyPath { get; set; }
    }
}
