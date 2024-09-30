using DiamondShop.Domain.Models.Promotions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Promotions
{
    public class DiscountDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsActive { get; set; }
        public string DiscountCode { get; set; }
        public int DiscountPercent { get; set; }
        public List<RequirementDto> DiscountReq { get; set; } = new();
    }
}
