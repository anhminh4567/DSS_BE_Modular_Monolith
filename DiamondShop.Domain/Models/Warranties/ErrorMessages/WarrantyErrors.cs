using DiamondShop.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Warranties.ErrorMessages
{
    public class WarrantyErrors
    {
        public static NotFoundError WarrantyNotFoundError = new NotFoundError("Không tìm thấy bảo hành");
        public static ValidationError ExistedWarrantyNameFound(string name) => new ValidationError($"Tên bảo hành (\"{name}\") đã tồn tại");
        public static ValidationError ExistedWarrantyCodeFound(string code) => new ValidationError($"Mã bảo hành (\"{code}\") đã tồn tại");

        public static ValidationError WrongJewelryError = new ValidationError("Sai loại bảo hành cho trang sức");
        public static ValidationError WrongDiamondError = new ValidationError("Sai loại bảo hành cho kim cương");
        public static ConflictError DeleteDefaultConflictError = new ConflictError("Không thể xóa bảo hành mặc định");
    }
}
