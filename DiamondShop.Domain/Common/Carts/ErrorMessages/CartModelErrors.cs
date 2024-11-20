using DiamondShop.Commons;
using DiamondShop.Domain.Models.AccountAggregate;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common.Carts.ErrorMessages
{
    public class CartModelErrors
    {
        public class CartProductError
        {
            public static Error UnknownPrice(int productIndex, CartProduct product) 
            {
                var addedMessage = "";  
                if (product.IsDiamond())
                    addedMessage = ($",loại kim cương với mã #{product.Diamond.SerialCode}");
                if(product.IsJewelry())
                    addedMessage = ($",loại trang sức với mã #{product.Jewelry.SerialCode}");
                return new Error($"Không rõ giá sản phẩm ở vị trí {productIndex}{addedMessage}");
            }
            public static Error NotFound(int productIndex, CartProduct product)
            {
                var addedMessage = "";
                if(product.IsDiamond())
                    addedMessage = ",loại kim cương";
                if (product.IsJewelry())
                    addedMessage = ",loại trang sức";
                return new Error($"Không tìm thấy sản phẩm ở vị trí {productIndex}{addedMessage}");
            }
            public static Error Sold(CartProduct cartProduct)
            {
                var baseMessage = "Sản phẩm đã được bán";
                if(cartProduct.IsDiamond())
                    baseMessage = "Kim cương đã được bán, mã là "+cartProduct.Diamond.SerialCode;
                if(cartProduct.IsJewelry())
                    baseMessage = "Trang sức đã được bán, mã là "+ cartProduct.Jewelry.SerialCode;
                return new Error($"{baseMessage}");
            }
            public static Error IncorrectStateForSale(CartProduct cartProduct)
            {
                var baseMessage = "trạng thái không hợp lệ, chỉ có sản phẩm active hay được lock cho bạn mới được mua";
                return new Error($"{baseMessage}");
            }
            public static Error NotLockForThisUser(CartProduct cartProduct, Account? currentAccount)
            {
                var baseMessage = "sản phẩm được lock cho người khác, bạn không có quyền mua";
                return new Error($"{baseMessage}");
            }
            public static ConflictError UnknownProductType => new ConflictError("Loại sản phẩm không xác định");
            public static ConflictError Duplicate => new ConflictError("Sản phẩm đã có trong giỏ");
            public class JewelryType
            {
                public static ConflictError DiamondNotCorrect => new ConflictError("Loại kim cương gắn vào không đúng");
            }
        }
    }
}

