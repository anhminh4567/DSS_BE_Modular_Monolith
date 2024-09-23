using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels
{
    public class JewelryModel : Entity<JewelryModelId>, IAggregateRoot
    {
        public string Name { get; set; }
        public JewelryModelCategoryId CategoryId { get; set; }
        public JewelryModelCategory Category { get; set; }
        public float? Width { get; set; }
        public float? Length { get; set; }
        public bool IsEngravable { get; set; }
        public bool IsRhodiumFinish { get; set; }
        public BackType? BackType { get; set; }
        public ClaspType? ClaspType { get; set; }
        public ChainType? ChainType { get; set; }
        public List<MainDiamondReq> MainDiamonds { get; set; } = new ();
        public List<SideDiamondReq> SideDiamonds { get; set; } = new();
        public List<SizeMetal> SizeMetals { get; set; } = new();
        /*public List<JewelryModelMedia> Medias { get; set; } = new();*/
        public JewelryModel() { }
    }
}
