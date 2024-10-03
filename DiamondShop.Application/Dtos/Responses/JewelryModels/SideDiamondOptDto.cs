using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.JewelryModels
{
    public class SideDiamondOptDto
    {
        public string Id { get; set; }
        public float CaratWeight { get; set; }
        public int Quantity { get; set; }
        public string SideDiamondReqId { get; set; }
        public SideDiamondReqDto SideDiamondReq { get; set; }
    }
}
