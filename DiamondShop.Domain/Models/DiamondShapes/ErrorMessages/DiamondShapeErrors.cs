using DiamondShop.Commons;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DiamondShapes.ErrorMessages
{
    public class DiamondShapeErrors
    {
        public static NotFoundError NotFoundError = new NotFoundError("Không tìm thấy hình dạng kim cương");
        public static ConflictError NotFancyShape = new ConflictError("Hình dạng không phải là Fancy Shape");
        public static ConflictError NotRoundShape = new ConflictError("Hình dạng không phải là Round Shape");
        public static Error Invalid = new Error("Hình dạng không hợp lệ");
    }
}
