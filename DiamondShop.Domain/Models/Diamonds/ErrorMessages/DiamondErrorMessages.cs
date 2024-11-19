using DiamondShop.Commons;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds.ErrorMessages
{
    public class DiamondErrorMessages
    {
        public static NotFoundError DiamondNotFoundError = new NotFoundError("Không tìm thấy kim cương");
        public static ConflictError DiamondExistError = new ConflictError("Kim cương đã tồn tại");
        public static ValidationError DiamondNotValid = new ValidationError("Kim cương không hợp lệ");
        public static ValidationError DiamondNotAvailable = new ValidationError("Kim cương không còn sẵn");
        public static ConflictError UnknownPrice = new ConflictError("Không tìm thấy giá kim cương, vui lòng liên hệ nhân viên để được báo giá riêng nếu có thể");
        public static Error NotInCorrectState = new Error("Kim cương không ở trạng thái yêu cầu");
        public static Error DeleteUnallowed = new Error("Không thể xóa kim cương này");

    }
}
