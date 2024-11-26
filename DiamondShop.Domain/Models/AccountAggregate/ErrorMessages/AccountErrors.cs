using DiamondShop.Commons;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.AccountAggregate.ErrorMessages
{
    public class AccountErrors 
    {
        public static NotFoundError AccountNotFoundError = new NotFoundError("Không tìm thấy tài khoản người dùng");
        public static NotFoundError TokenNotFoundError = new NotFoundError("Không tìm thấy token của người dùng");
        public static NotFoundError PrincipalNotFoundError = new NotFoundError("Không thể xác thực người dùng");
        public static ValidationError TokenExpiredError = new ValidationError("Token đã hết hạn");
        public static ValidationError GoogleAccountFailureError = new ValidationError("Không thể đăng nhập vào Google, thử lại sau");
        public static ValidationError IncorrectUserNamePassword =new ValidationError("Sai tên đăng nhập hoặc mật khẩu");
        public static ConflictError TokenNotMatchError = new ConflictError("Token người dùng không khớp");
        public static ConflictError LockAccount = new ConflictError("Tài khoản đã bị khóa");
        public static ConflictError AccountIsNotActive = new ConflictError("Tài khoản chưa được kích hoạt");
        public static ConflictError GoogleInvalidError = new ConflictError("Thông tin xác thực không hợp lệ");
        public class Register
        {
            public static ConflictError UserExist = new ConflictError("Tài khoản đã tồn tại");
            public static ConflictError EmailExist = new ConflictError("Email đã tồn tại");
            public static ConflictError PasswordConflict = new ConflictError("Mật khẩu không khớp");
            public static ConflictError IdentityCreateFail = new ConflictError("Lỗi tạo thông tin tài khoản");
            public static ConflictError LoginCreateFail = new ConflictError("Lỗi tạo đăng nhập");
        }
        public class Login
        {
            public static Error LoginFail = new Error("Lỗi đăng nhập, nhập đúng email và password ");
            public static Error LoginLimitReached = new Error("Đăng nhập quá số lần cho phép, vui lòng thử lại sau");
            public static NotFoundError NoSchemeFoundError = new NotFoundError("Không tìm thấy bên cung cấp tài khoản");
        }
        public class Profile
        {
            public static Error MaxAddressReached (int max) => new Error($"Số lượng địa chỉ tối đa là {max}");
            public static Error AddressNotFound = new Error("Không tìm thấy địa chỉ");
            public static Error InvalidUpdateData = new Error("Dữ liệu cập nhật không hợp lệ");
        }
    }
}
