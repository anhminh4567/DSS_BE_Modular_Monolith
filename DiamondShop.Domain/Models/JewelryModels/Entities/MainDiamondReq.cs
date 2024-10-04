using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class MainDiamondReq : Entity<MainDiamondReqId>
    {
        public JewelryModelId ModelId { get; set; }
        public List<MainDiamondShape> Shapes { get; set; } = new ();
        public SettingType SettingType { get; set; }
        public int Quantity { get; set; }
        public MainDiamondReq() { }
        public static MainDiamondReq Create(JewelryModelId modelId, SettingType settingType, int quantity, MainDiamondReqId givenId = null)
        {
            return new MainDiamondReq()
            {
                Id = givenId is null ? MainDiamondReqId.Create() : givenId,
                SettingType = settingType,
                Quantity = quantity
            };
        } 
    }
}
