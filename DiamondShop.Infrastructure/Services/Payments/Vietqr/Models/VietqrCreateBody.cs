using System.ComponentModel.DataAnnotations;

namespace DiamondShop.Infrastructure.Services.Payments.Vietqr.Models
{
    public class VietqrCreateBody
    {
        [Required]
        public string bankCode { get; set; } // Mã ngân hàng của tài khoản.

        [Required]
        public string bankAccount { get; set; } // Tài khoản ngân hàng tạo mã thanh toán VietQR.

        [Required]
        public string userBankName { get; set; } // Họ tên chủ tài khoản. Không dấu tiếng Việt.

        [Required]
        public string content { get; set; } // Nội dung chuyển tiền. Tối đa 70 ký tự, không dấu.

        [Required]
        public int qrType { get; set; } // Loại mã thanh toán cần tạo.

        public long? amount { get; set; } // Số tiền cần thanh toán.

        public string orderId { get; set; } // Mã ID giao dịch bên đối tác.

        public string transType { get; set; } // Phân loại giao dịch (ghi nợ/ghi có).

        public string terminalCode { get; set; } // Mã cửa hàng/điểm bán hàng.

        public string ServiceCode { get; set; } // Mã sản phẩm/dịch vụ được thanh toán.

        public string SubTerminalCode { get; set; } // Mã cửa hàng/điểm bán phụ.

        public string Signature { get; set; } // Chữ ký.

        public string AdditionalInfo { get; set; } // Các tham số truyền thêm.
        public string urlLink { get; set; }
    }
}
