using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds.DashboardDto
{
    public class DiamondBestSellingShapeDto
    {
        public int TotalSold { get; set; }  
        public int TotalInStock { get; set; }
        public int TotalActive { get; set; }    
        public int TotalInactive { get; set; }
        public int TotalLockForUser { get; set; }
        public int TotalLock { get; set; }
        public decimal TotalRevenueForThisShape { get; set; } = 0;
        public DiamondShapeDto Shape { get; set; }
    }

}
