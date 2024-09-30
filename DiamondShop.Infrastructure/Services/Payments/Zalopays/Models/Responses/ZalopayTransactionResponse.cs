using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Zalopays.Models.Responses
{
    public class ZalopayTransactionResponse
    {
        public int return_code { get; set; }            // 1: Thành công
                                                        // 2: Thất bại
                                                        // 3: Đơn hàng chưa thanh toán hoặc giao dịch đang xử lý
        public string return_message { get; set; }      // Thông tin trạng thái đơn hàng
        public int? sub_return_code { get; set; }       // Mã trạng thái chi tiết
        public string sub_return_message { get; set; }  // Thông tin chi tiết trạng thái đơn 
        public bool is_processing { get; set; }         // Trạng thái xử lý
        public long? amount { get; set; }               // Số tiền ứng dụng nhận được (chỉ có ý nghĩa khi thanh toán thành công)
        public long? discount_amount { get; set; }      // Số tiền giảm giá

        public long? zp_trans_id { get; set; }          // Mã giao dịch của ZaloPay
    }
}
