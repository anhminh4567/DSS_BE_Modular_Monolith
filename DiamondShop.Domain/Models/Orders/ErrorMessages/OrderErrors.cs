using DiamondShop.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.ErrorMessages
{
    public class OrderErrors
    {
        public static NotFoundError OrderNotFoundError = new NotFoundError("Không tìm thấy đơn hàng");
        public static NotFoundError OrderStatusNotFoundError = new NotFoundError("Không tìm thấy tình trạng đơn hàng");
        public static ValidationError UncancellableError = new ValidationError("Đơn hàng không thể được hủy");
        public static ValidationError RefundedError = new ValidationError("Đơn hàng đã được hoàn tiền trước đó");
        public static ValidationError MaxRedeliveryError = new ValidationError("Đơn hàng đã hết lượt giao lại");
        public static ValidationError NoPermissionToViewError = new ValidationError("Tài khoản không có quyền xem đơn hàng");
        public static ValidationError NoPermissionToCancelError = new ValidationError("Tài khoản không có quyền hủy đơn hàng");
        public static ValidationError NoDelivererAssignedError = new ValidationError("Đơn hàng chưa được giao cho nhân viên giao hàng");
        public static ValidationError NoDelivererToAssignError = new ValidationError("Không có nhân viên giao hàng để chuyển giao");
        public static ValidationError OnlyDelivererAllowedError = new ValidationError("Chỉ có nhân viên giao hàng mới được thực hiện hành động");
        public static ValidationError UnproceedableError = new ValidationError("Đơn hàng không thể được tiếp tục");
    }
}
