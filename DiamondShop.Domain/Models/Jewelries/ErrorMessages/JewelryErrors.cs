using DiamondShop.Commons;
using DiamondShop.Domain.Common.Enums;

namespace DiamondShop.Domain.Models.Jewelries.ErrorMessages
{
    public class JewelryErrors
    {
        public static NotFoundError JewelryNotFoundError = new NotFoundError("Không tìm thấy trang sức");
        public static ConflictError CodeInUseError = new ConflictError("Mã trang sức đã tồn tại");
        public static ConflictError JewelryInUseError = new ConflictError("Trang sức đang được sử dụng");
        public static ConflictError IsSold = new ConflictError("Trang sức đã được bán");
        public static ConflictError IsPreOrder = new ConflictError("Trang sức đang ở trạng thái đặt trước");
        public static ConflictError IsLocked = new ConflictError("Trang sức đã được khóa");
        public static ConflictError IsInactive = new ConflictError("Trang sức không còn hoạt động");
        public static ConflictError InCorrectState(string? message = null) 
        {
            if (message is null)
                return new ConflictError("Trạng thái trang sức không hợp lệ");
            return new ConflictError($"Trạng thái trang sức không hợp lệ, {message}");
        } 
        public class SideDiamond
        {
            public static ValidationError UnsupportedSideDiamondError = new ValidationError("Carat của kim cương tấm không được hỗ trợ");

        }
        public class Review
        {
            public static NotFoundError ReviewNotFoundError = new NotFoundError("Không tìm thấy đánh giá");
            public static NotFoundError ReviewJewelryNotFoundError = new NotFoundError("Không tìm thấy trang sức của đánh giá");
            public static ValidationError NoPermissionError = new ValidationError("Tài khoản không phải chủ đánh giá");
            public static ValidationError AlreadyHiddenError = new ValidationError("Đánh giá đã được gỡ");
        }
    }
}
