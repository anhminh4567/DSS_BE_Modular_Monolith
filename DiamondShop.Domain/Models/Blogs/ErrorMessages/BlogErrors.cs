using DiamondShop.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Blogs.ErrorMessages
{
    public class BlogErrors
    {
        public static NotFoundError BlogNotFoundError = new NotFoundError("Không tìm thấy bài viết");
        public static ValidationError NoPermissionError = new ValidationError("Tài khoản không phải chủ bài viết");
    }
}
