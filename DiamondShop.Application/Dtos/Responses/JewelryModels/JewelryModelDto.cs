using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.JewelryModels
{
    public class JewelryModelDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public JewelryModelCategoryDto Category { get; set; }
        public float? Width { get; set; }
        public float? Length { get; set; }
        public bool IsEngravable { get; set; }
        public bool IsRhodiumFinish { get; set; }
        public string? BackType { get; set; }
        public string? ClaspType { get; set; }
        public string? ChainType { get; set; }
        public List<MainDiamondReqDto> MainDiamonds { get; set; } = new();
        public List<SideDiamondReqDto> SideDiamonds { get; set; } = new();
        public List<SizeMetalDto> SizeMetals { get; set; } = new();
    }
}
