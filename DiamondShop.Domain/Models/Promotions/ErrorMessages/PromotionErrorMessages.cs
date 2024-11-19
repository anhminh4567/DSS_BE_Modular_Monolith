using DiamondShop.Commons;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.ErrorMessages
{
    public class PromotionErrorMessages
    {
        public static NotFoundError PromotionNotFoundError = new NotFoundError("Không tìm thấy khuyến mãi");
        public static ConflictError PromotionExistError = new ConflictError("Khuyến mãi đã tồn tại");
        public static ValidationError PromotionNotValid = new ValidationError("Khuyến mãi không hợp lệ");
        public static Error ChangeDataNotValid = new Error("Dữ liệu thay đổi không hợp lệ");
        public static Error NotInCorrectState = new Error("Khuyến mãi không ở trạng thái yêu cầu để thực hiện hành động này");
        public static Error DeleteUnallowed = new Error("Không thể xóa khuyến mãi này");
        public class GiftError
        {
            public static NotFoundError GiftNotFoundError = new NotFoundError("Không tìm thấy quà tặng");
            public static ValidationError GiftNotValid = new ValidationError("Quà tặng không hợp lệ");
            public static Error NotInCorrectState = new Error("Quà tặng không ở trạng thái yêu cầu để thực hiện hành động này");
            public static Error DeleteUnallowed = new Error("Không thể xóa quà tặng này");
            public static ValidationError ValidationError = new ValidationError("Lỗi dữ liệu quà tặng, không thể tạo hay xóa");
        }
        public class RequirementError
        {
            public static NotFoundError RequirementNotFoundError = new NotFoundError("Không tìm thấy yêu cầu");
            public static ValidationError RequirementNotValid = new ValidationError("Yêu cầu không hợp lệ");
            public static Error NotInCorrectState = new Error("Yêu cầu không ở trạng thái yêu cầu để thực hiện hành động này");
            public static Error DeleteUnallowed = new Error("Không thể xóa yêu cầu này");
            public static ValidationError ValidationError = new ValidationError("Lỗi dữ liệu yêu cầu, không thể tạo hay xóa");
        }
    }
}
