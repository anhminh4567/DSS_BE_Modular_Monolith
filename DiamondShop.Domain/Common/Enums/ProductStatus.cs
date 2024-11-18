using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common.Enums
{
    public enum ProductStatus
    {
        Active = 1, Sold = 2, Locked = 3, Inactive = 4, LockForUser = 5, PreOrder = 6
    }
    public static class ProductStatusHelper
    {
        public static string GetStatusName(ProductStatus status)
        {
            return status switch
            {
                ProductStatus.Active => "đang bán",
                ProductStatus.Sold => "đã bán",
                ProductStatus.Locked => "khóa cho khách hoặc cho trang sức",
                ProductStatus.Inactive => "không bán",
                ProductStatus.LockForUser => "khóa cho khách mua",
                ProductStatus.PreOrder => "Sản phẩm đặt trước",
                _ => throw new Exception("invalid state")
            }; 
        }
        public static List<ProductStatus> GetAllStatus()
        {
            return new List<ProductStatus> { ProductStatus.Active, ProductStatus.Sold, ProductStatus.Locked, ProductStatus.Inactive, ProductStatus.LockForUser, ProductStatus.PreOrder };
        }
    }
}
