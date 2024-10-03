using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.JewelryModels
{
    public class MainDiamondReqDto
    {
        public string Id { get; set; }
        public string ModelId { get; set; }
        public List<MainDiamondShapeDto> Shapes { get; set; } = new();
        public SettingType SettingType { get; set; }
        public int Quantity { get; set; }
    }
}
