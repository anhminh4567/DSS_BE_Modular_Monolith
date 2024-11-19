using DiamondShop.Commons;

namespace DiamondShop.Domain.Models.Jewelries.ErrorMessages
{
    public class JewelryErrors
    {
        public static NotFoundError JewelryNotFoundError = new NotFoundError("Không tìm thấy trang sức");
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
