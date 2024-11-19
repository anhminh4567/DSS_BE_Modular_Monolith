using DiamondShop.Commons;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DiamondPrices.ErrorMessages
{
    public class DiamondPriceErrorMessages
    {
        public static NotFoundError DiamondPriceNotFoundError = new NotFoundError("Không tìm thấy giá kim cương");
        public static ValidationError DiamondPriceExistError = new ValidationError("Giá kim cương đã tồn tại");
        public static ValidationError DiamondPriceNotValid = new ValidationError("Giá kim cương không hợp lệ");
        public static NotFoundError UnsupportedCaratRange = new NotFoundError("Không hỗ trợ dải carat này");
        public static NotFoundError UnsupportedColorRange = new NotFoundError("Không hỗ trợ dải màu này");
        public static NotFoundError UnsupportedClarityRange = new NotFoundError("Không hỗ trợ dải độ trong này");
    }
}
