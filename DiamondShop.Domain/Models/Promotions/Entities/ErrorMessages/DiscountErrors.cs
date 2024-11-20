using DiamondShop.Commons;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.Entities.ErrorMessages
{
    public class DiscountErrors
    {
        public static NotFoundError NotFound = new NotFoundError("Không tìm thấy Giảm giá ");
        public static ConflictError Exist(string? detail = null) 
        {
            if (detail != null)
                return new ConflictError("Giảm giá đã tồn tại, " + detail);
            return new ConflictError("Giảm giá đã tồn tại");
        }
        public static ValidationError DiscountNotValid(string? extraMessage, Dictionary<string, object>? errors)
        {
            if (extraMessage != null && errors != null)
                return new ValidationError("Giảm giá không hợp lệ, " + extraMessage, errors);
            if (extraMessage != null)
                return new ValidationError("Giảm giá không hợp lệ, " + extraMessage);
            if (errors != null)
                return new ValidationError("Giảm giá không hợp lệ", errors);
            return new ValidationError("Giảm giá không hợp lệ");
        }
        public static Error ChangeDataNotValid = new Error("Dữ liệu thay đổi không hợp lệ");
        public static Error NotInCorrectState = new Error("Giảm giá không ở trạng thái yêu cầu để thực hiện hành động này");
        public static Error DeleteUnallowed(string? detail) 
        {
            if(detail != null)
                return new Error("Không thể xóa Giảm giá này, lý do " + detail);
            return new Error("Không thể xóa Giảm giá này");
        } 
        public static Error OrderTargetNotAllowed => new Error("Không tạo discount với đối tượng đơn hàng, chỉ có thể tạo với jewerly và diamond vói loại unit type là Phần trăm(%)");
        public static Error UnitTypeNotAllowed => new Error("Không tạo discount với loại unit type này, chỉ có thể tạo với loại unit type là Phần trăm(%)");
    }
}
