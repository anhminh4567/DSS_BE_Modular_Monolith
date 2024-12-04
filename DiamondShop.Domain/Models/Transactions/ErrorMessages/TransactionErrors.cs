using DiamondShop.Commons;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DiamondShop.Domain.Models.Transactions.ErrorMessages
{
    public class TransactionErrors
    {
        public static NotFoundError TransactionNotFoundError = new NotFoundError("Không tìm thấy giao dịch");
        public static ConflictError TransactionExistError = new ConflictError("Giao dịch đã tồn tại");
        public static ValidationError TransactionNotValid = new ValidationError("Giao dịch không hợp lệ");
        public static Error ChangeDataNotValid = new Error("Dữ liệu thay đổi không hợp lệ");
        public static Error NotInCorrectState = new Error("Giao dịch không ở trạng thái yêu cầu để thực hiện hành động này");
        public static Error DeleteUnallowed = new Error("Không thể xóa giao dịch này");
        public class PaygateError
        {
            public static NotFoundError PaygateNotFoundError = new NotFoundError("Không tìm thấy cổng thanh toán");
            public static ValidationError PaygateDataNotValid = new ValidationError("Cổng thanh toán không hợp lệ");
            public static ValidationError MaxTransactionError(string paymentName, long maxAmount) => new ValidationError($"Cổng thanh toán {paymentName} chỉ hỗ trợ tối đa {maxAmount.ToString("{0:#,##0}")} VND trên giao dịch");
            public static Error PaygateRefuseToWork = new Error("Cổng thanh toán không hoạt động");
            public static NotFoundError RefundNotFound = new NotFoundError("Không tìm thấy yêu cầu hoàn tiền");
            public static Error RefundRefused = new Error("Yêu cầu hoàn tiền bị từ chối");
            public static Error RefundNotValidData = new Error("Dữ liệu yêu cầu hoàn tiền không hợp lệ");
        }
        public class TransferError
        {
            public static NotFoundError EvidenceNotFoundError = new NotFoundError("Không tìm thấy bằng chứng giao dịch cũ");
            public static ValidationError VerifiedError = new ValidationError("Giao dịch đã được xử lý");
            public static ValidationError EvidenceUnchangableError = new ValidationError("Không thể thay đổi bằng chứng giao dịch");
        }
        public class PaymentMethodErrors
        {
            public static NotFoundError NotFoundError = new NotFoundError("Không tìm thấy phương thức thanh toán");
            public static ConflictError InCorrectForOnlinePayment = new ConflictError("Phương thức thanh toán không hợp lệ cho chuyển khoản online");
        }
    }
}
