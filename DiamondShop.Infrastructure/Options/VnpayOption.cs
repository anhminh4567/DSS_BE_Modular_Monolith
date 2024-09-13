using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options
{
    internal class VnpayOption
    {
        public static string Section = "VnpaySetting";
        public string Vnp_TmnCode { get; set; }
        public string Vnp_HashSecret { get; set; }
        public string Vnp_Url { get; set; }
        public string Vnp_Query_Url { get; set; }
        public string Vnp_Return_Url { get; set; }
        public int Vnp_Max_Per_Transaction_VND { get; set; }
        public int Vnp_Min_Per_Transaction_VND { get; set; }
        public int Vnp_Payment_Timeout_Minute { get; set; }
        public string Vnp_DateTime_Format { get; set; }
    }
}
