using DiamondShop.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.CustomizeRequests.ErrorMessages
{
    public class CustomizeRequestErrors
    {
        public static NotFoundError CustomizeRequestNotFoundError = new NotFoundError("Không tìm thấy yêu cầu thiết kế");
        public static ValidationError NoPermissionError = new ValidationError("Tài khoản không phải chủ yêu cầu thiết kế");
        public static ValidationError UnacceptedCheckoutError = new ValidationError("Yêu cầu thiết kế phải được chấp thuận trước khi thanh toán");
        public static ValidationError UnproceedableError = new ValidationError("Yêu cầu thiết kế chưa được xử lý");
        public static ValidationError UnpricableError = new ValidationError("Yêu cầu thiết kế đã được tính giá");
        public static ValidationError UnrequestableError = new ValidationError("Yêu cầu thiết kế chưa được tính giá");
        public static ValidationError UnacceptableError = new ValidationError("Yêu cầu thiết kế không thể được chấp thuận");
        public static ValidationError UnrejectableError = new ValidationError("Yêu cầu thiết kế không thể được từ chối");
        public static ValidationError UncancelableError = new ValidationError("Yêu cầu thiết kế không thể được hủy");
        public static ConflictError UnchosenSideDiamondOptError = new ConflictError("Yêu cầu thiết kế chưa chọn kim cương tấm");

        public static ConflictError ExpiredError = new ConflictError("Yêu cầu thiết kế này đã hết hạn");
        public static ConflictError ExistedOrderError = new ConflictError("Yêu cầu thiết kế này đã được xử lý");
        public class DiamondRequest
        {
            public static NotFoundError DiamondRequestNotFoundError = new NotFoundError("Không tìm thấy yêu cầu kim cương");
            public static NotFoundError OldAttachedDiamondNotFoundError = new NotFoundError("Không tìm thấy kim cương đã ấn định trước đó");
            public static ValidationError InvalidChangingDiamondStatusError = new ValidationError("Chỉ có thể đổi kim cương trước khi người dùng xác nhận");
            public static ConflictError UnchosenMainDiamondError(int index) => new ConflictError($"Yêu cầu kim cương số {index} chưa được chọn kim cương");
            public static ConflictError ConflictedDiamondIdError = new ConflictError("Kim cương trùng lặp kim cương đã ấn định");
        }

    }
}
