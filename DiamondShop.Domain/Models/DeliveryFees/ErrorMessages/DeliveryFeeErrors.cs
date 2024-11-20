using DiamondShop.Commons;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DeliveryFees.ErrorMessages
{
    public class DeliveryFeeErrors
    {
        public static NotFoundError DeliveryFeeNotFoundError = new NotFoundError("Không tìm thấy phí vận chuyển");
        public static ValidationError DeliveryFeeExistError = new ValidationError("Phí vận chuyển đã tồn tại");
        public static ValidationError DeliveryFeeNotValid = new ValidationError("Phí vận chuyển không hợp lệ");
        public static Error NotSupported = new Error("khu Vực không được hỗ trợ");
    }
}
