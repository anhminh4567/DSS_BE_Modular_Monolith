using DiamondShop.Commons;

namespace DiamondShop.Domain.Models.JewelryModels.ErrorMessages
{
    public class JewelryModelErrors
    {
        public static NotFoundError JewelryModelNotFoundError = new NotFoundError("Không tìm thấy mẫu trang sức");
        public static ValidationError NoEngravingError = new ValidationError("Mẫu trang sức không hỗ trợ khắc chữ");
        public static ValidationError SideDiamondNeededError = new ValidationError("Mẫu trang sức yêu cầu kim cương tấm");
        public static ValidationError ExistedModelNameFound(string name) => new ValidationError($"Tên mẫu trang sức (\"{name}\") đã tồn tại");
        public static ValidationError ExistedModelCodeFound(string code) => new ValidationError($"Mã mẫu trang sức (\"{code}\") đã tồn tại");
        public static ConflictError JewelryModelInUseError = new ConflictError("Mẫu trang sức đang được sử dụng");
        public class Category
        {
            public static NotFoundError JewelryModelCategoryNotFoundError = new NotFoundError("Không tìm thấy loại trang sức");
            public static ConflictError DeleteDefaultJewelryModelCategoryError = new ConflictError("Không thể xóa loại trang sức mặc định");
            public static ConflictError DeleteJewelryModelCategoryInUseError = new ConflictError("Không thể xóa loại trang sức đang được sử dụng");
        }
        public class SizeMetal
        {
            public static NotFoundError SizeMetalNotFoundError = new NotFoundError("Không tìm thấy kích thước cho mẫu trang sức thuộc kim loại này");
            public static ConflictError SizeMetalInUseConflictError = new ConflictError("Kích thước cho mẫu trang sức thuộc kim loại này vẫn đang được sử dụng");
        }
        public class SideDiamond
        {
            public static NotFoundError SideDiamondOptNotFoundError = new NotFoundError("Không tìm thấy lựa chọn kim cương tấm");
            public static NotFoundError NoCriteriaFound(int index) => new NotFoundError($"Không tìm thấy tiêu chí kim cương tấm số {index}");
            public static ValidationError UnsupportedSideDiamondCaratError = new ValidationError("Carat của kim cương tấm không được hỗ trợ");
            public static ValidationError NoSideDiamondSupportError = new ValidationError("Mẫu trang sức không hỗ trợ kim cương tấm");
            public static ConflictError SideDiamondOptAlreadyExistError = new ConflictError("Lựa chọn kim cương tấm đã tồn tại");
            public static ConflictError SideDiamondOptInUseConflictError = new ConflictError("Lựa chọn kim cương tấm vẫn đang được sử dụng");
            public static ConflictError UnpricedSideDiamondOptError = new ConflictError("Lựa chọn kim cương tấm chưa có giá");
            public static ConflictError SideDiamondOptMinimumError = new ConflictError("Mẫu trang sức phải giữ lại ít nhất một lựa chọn kim cương tấm");
            public static ConflictError ModelUnsupportedError = new ConflictError("Mẫu trang sức không hỗ trợ kim cương tấm");
            public static ConflictError ModelMaximumOptionError(int max) => new ConflictError($"Mỗi mẫu trang sức chỉ được chứa tối đa {max} lựa chọn kim cương tấm");
        }
        public class MainDiamond
        {
            public static ValidationError MainDiamondCountError(int quantity) => new ValidationError($"Mẫu trang sức yêu cầu {quantity} kim cương");
            public static ValidationError MismatchCaratError(int index) => new ValidationError($"Mẫu trang sức không hỗ trợ carat của kim cương số {index}");
            public static ValidationError MismatchShapeError(int index) => new ValidationError($"Mẫu trang sức không hỗ trợ hình dạng của kim cương số {index}");
            public static ValidationError MismatchDiamondListError(int index) => new ValidationError($"Mẫu trang sức không hỗ trợ hình dạng của kim cương số {index}");
            public static ConflictError ModelMaximumOptionError(int max) => new ConflictError($"Mỗi mẫu trang sức chỉ được chứa tối đa {max} kim cương chính");
            public static ConflictError ModelUnsupportedError = new ConflictError("Mẫu trang sức không hỗ trợ kim cương chính");
        }
    }
}
