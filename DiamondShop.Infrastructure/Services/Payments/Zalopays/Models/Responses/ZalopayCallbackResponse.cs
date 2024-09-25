using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Zalopays.Models.Responses
{

    public class ZalopayCallbackResponse
    {
        public int return_code { get; set; }
        public string return_message { get; set; } 
    }
    public class ZalopayCallbackReturnCode
    {
        public const int Thanh_Cong = 1;
        public const int Trung_Giao_Dich_That_bai = 2;
        public const int That_Bai = -1;
    }
}
