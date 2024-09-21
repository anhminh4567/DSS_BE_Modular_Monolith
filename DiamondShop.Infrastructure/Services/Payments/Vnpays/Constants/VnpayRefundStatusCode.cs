using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Vnpays.Constants
{
    public static class VnpayRefundStatusCode
    {
        public static string Success => "Yêu cầu thành công"; //00
        public static string InvalidPartnerCode => "Mã định danh kết nối không hợp lệ (kiểm tra lại TmnCode)";//02
        public static string InvalidDataFormat => "Dữ liệu gửi sang không đúng định dạng";//03
        public static string TransactionNotFound => "Không tìm thấy giao dịch yêu cầu hoàn trả";//91
        public static string AlreadyRefunded => "Giao dịch đã được gửi yêu cầu hoàn tiền trước đó. Yêu cầu này VNPAY đang xử lý";//91
        public static string TransactionFailed => "Giao dịch này không thành công bên VNPAY. VNPAY từ chối xử lý yêu cầu";//94
        public static string InvalidChecksum => "Checksum không hợp lệ";//95
        public static string OtherErrors => "Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê)";//99
    }
}
