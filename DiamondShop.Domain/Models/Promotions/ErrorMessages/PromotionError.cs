using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions.Enum;
using FluentResults;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.ErrorMessages
{
    public class PromotionError
    {
        public static NotFoundError NotFound = new NotFoundError("Không tìm thấy khuyến mãi");
        public static ConflictError ExistError = new ConflictError("Khuyến mãi đã tồn tại");
        public static ValidationError PromotionNotValid(string? extraMessage, Dictionary<string, object>? errors) 
        {
            if (extraMessage != null && errors != null)
                return new ValidationError("Khuyến mãi không hợp lệ, " + extraMessage, errors);
            if (extraMessage != null)
                return new ValidationError("Khuyến mãi không hợp lệ, " + extraMessage);
            if (errors != null)
                return new ValidationError("Khuyến mãi không hợp lệ", errors);
            return new ValidationError("Khuyến mãi không hợp lệ");
        }
        public static Error ChangeDataNotValid = new Error("Dữ liệu thay đổi không hợp lệ");
        public static Error NotInCorrectState = new Error("Khuyến mãi không ở trạng thái yêu cầu để thực hiện hành động này");
        public static Error DeleteUnallowed = new Error("Không thể xóa khuyến mãi này, chỉ có thể xóa nếu nó cancelled, expired, scheduled");
        public static Error RequirentTypeLimit(TargetType type, int limit)
        {
            return new Error($"Không thể thêm yêu cầu mới, đã đạt giới hạn {type} là {limit}");
        }
        public static Error GiftTypeLimit(TargetType type, int limit)
        {
            return new Error($"Không thể thêm yêu cầu mới, đã đạt giới hạn {type} là {limit}");
        }
        public class GiftError
        {
            public static NotFoundError NotFound = new NotFoundError("Không tìm thấy quà tặng");
            public static ValidationError GiftNotValid(string? extraMessage, Dictionary<string, object>? errors)
            {
                if (extraMessage != null && errors != null)
                    return new ValidationError("Quà tặng không hợp lệ, " + extraMessage, errors);
                if (extraMessage != null)
                    return new ValidationError("Quà tặng không hợp lệ, " + extraMessage);
                if (errors != null)
                    return new ValidationError("Quà tặng không hợp lệ", errors);

                return new ValidationError("Quà tặng không hợp lệ");
            } 
            public static Error NotInCorrectState = new Error("Quà tặng không ở trạng thái yêu cầu để thực hiện hành động này");
            public static Error DeleteUnallowed = new Error("Không thể xóa quà tặng này");
            public static ValidationError ValidationError = new ValidationError("Lỗi dữ liệu quà tặng, không thể tạo hay xóa");
            public static Error CountIsZero = new Error("Số lượng quà tặng phải lớn hơn 0");
        }
        public class RequirementError
        {
            public static NotFoundError NotFound = new NotFoundError("Không tìm thấy yêu cầu");
            public static ValidationError RequirementNotValid(string? extraMessage, Dictionary<string, object>? errors)
            {
                if (extraMessage != null && errors != null)
                    return new ValidationError("Yêu cầu không hợp lệ, " + extraMessage, errors);
                if (extraMessage != null)
                    return new ValidationError("Yêu cầu không hợp lệ, " + extraMessage);
                if (errors != null)
                    return new ValidationError("Yêu cầu không hợp lệ", errors);
                return new ValidationError("Yêu cầu không hợp lệ");
            }

            public static Error NotInCorrectState = new Error("Yêu cầu không ở trạng thái yêu cầu để thực hiện hành động này");
            public static Error DeleteUnallowed(string? detail) 
            {
                if(detail != null)
                    return new Error("Không thể xóa yêu cầu này, lý do " + detail);
                return new Error("Không thể xóa yêu cầu này");
            }
            public static Error CountIsZero = new Error("Số lượng requirement phải lớn hơn 0");
        }
        public class ApplyingError
        {
            public static Error NotActiveToUse = new Error("Khuyến mãi không ở trạng thái sử dụng");
            public static Error NotMeetRequirement = new Error("Khuyến mãi không đáp ứng yêu cầu");
            public static ConflictError AlreadyAppliedPromo = new ConflictError("Khuyến mãi đã được áp dụng");
        }
    }
}
