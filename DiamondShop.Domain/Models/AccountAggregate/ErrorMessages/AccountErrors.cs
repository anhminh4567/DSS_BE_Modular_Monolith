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
        public static NotFoundError AccountNotFoundError = new NotFoundError("Không tìm thấy tài khoản người dùng email ádfsadsadfsdf");
        public static ValidationError IncorrectUserNamePassword =new ValidationError("Sai tên đăng nhập hoặc mật khẩu");
        public static ConflictError LockAccount = new ConflictError("Tài khoản đã bị khóa");
        public static ConflictError AccountIsNotActive = new ConflictError("Tài khoản chưa được kích hoạt");
        public class Register
        {
            public static ConflictError UserExist = new ConflictError("Tài khoản đã tồn tại");
            public static ConflictError EmailExist = new ConflictError("Email đã tồn tại");
            public static ConflictError PasswordConflic = new ConflictError("Mật khẩu không khớp");
        }
    }
}
