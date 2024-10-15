using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class LocationRules
    {
        public string OriginalLocationName { get; set; } = "Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Hồ Chí Minh 700000, Vietnam"; // lay fpt lam goc
        public string OrignalAddress { get; set; } = "Lô E2a-7, Đường D1, Đ. D1";
        public string OrignalWard { get; set; } = "Long Thạnh Mỹ";
        public string OrignalDistrict { get; set; } = "Thành Phố Thủ Đức";
        public string OriginalProvince { get; set; } = "Hồ Chí Minh";
        public string OrinalPlaceId { get; set; } = "ChIJsQdrFzEndTERXq6bN0uyUrc";
        public string OriginalLongLat { get; set; } = "10.8411276,106.809883";
    }
}
