using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds.DashboardDto
{
    public class DiamondBestSellingCaratRangePerShapeDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public DiamondShapeDto Shape { get; set; }
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<string> SoldDiamondIds { get; set; } = new List<string>();
    }
}
