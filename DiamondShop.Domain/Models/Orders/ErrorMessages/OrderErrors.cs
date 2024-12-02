using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using FluentResults;
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
        public static ValidationError UnpaidError = new ValidationError("Đơn hàng chưa được thanh toán hết");
        public static ValidationError UncancellableError = new ValidationError("Đơn hàng không thể được hủy");
        public static ValidationError RefundedError = new ValidationError("Đơn hàng đã được hoàn tiền trước đó");
        public static ValidationError MaxRedeliveryError = new ValidationError("Đơn hàng đã hết lượt giao lại");
        public static ValidationError NoPermissionError = new ValidationError("Tài khoản không có quyền truy cập");
        public static ValidationError NoPermissionToViewError = new ValidationError("Tài khoản không có quyền xem đơn hàng");
        public static ValidationError NoPermissionToCancelError = new ValidationError("Tài khoản không có quyền hủy đơn hàng");
        public static ValidationError NoDelivererAssignedError = new ValidationError("Đơn hàng chưa được giao cho nhân viên giao hàng");
        public static ValidationError NoDelivererToAssignError = new ValidationError("Không có nhân viên giao hàng để chuyển giao");
        public static ValidationError OnlyDelivererAllowedError = new ValidationError("Chỉ có nhân viên giao hàng mới được thực hiện hành động");
        public static ValidationError UnproceedableError = new ValidationError("Đơn hàng không thể được tiếp tục");
        public static ConflictError NotValidForCODType = new ConflictError("Đơn hàng không hợp lệ cho phương thức thanh toán COD, gía trị đơn hàng phải trên " + OrderPaymentRules.Default.MinAmountForCOD + " thì mới được dùng COD");
        //Chuyển khoản
        public static ValidationError UnTransferableError = new ValidationError("Đơn hàng không thể được chuyển khoản");
        public static ValidationError ExpiredTimeDueError = new ValidationError("Đơn hàng đã qua thời hạn chuyển khoản nữa");
        public class Refund
        {
            public static ConflictError ExistVerifyingTransferError = new ConflictError("Đơn hàng còn giao dịch chưa xác thực");
        }
        public class LogError
        {
            public static NotFoundError NotFound = new NotFoundError("Không tìm thấy log đơn hàng");
            public static NotFoundError ParentLogNotFound = new NotFoundError("Không tìm thấy log cha của log đơn hàng, có thể đơn chưa process hay chưa được giao");
        }
        public static Error NotComplete(string? detail = null)
        {
            if (detail is not null)
                return new Error("Đơn hàng chưa hoàn thành, " + detail);
            return new Error("Đơn hàng chưa hoàn thành");
        }
        public static Error LackEvidenceToCompleteDeliver = new Error("Thiếu chứng cứ để hoàn thành giao hàng");
    }
}
