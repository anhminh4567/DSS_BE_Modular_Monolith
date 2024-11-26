using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Enum
{
    public enum OrderStatus
    {
        Pending = 1, Processing = 2, Rejected = 3, Cancelled = 4, Prepared = 5, Delivering = 6, Delivery_Failed = 7, Success = 8
    }

    public static class OrderStatusExtension
    {
        public static string ToFriendlyString(this OrderStatus me)
        {
            switch (me)
            {
                case OrderStatus.Pending:
                    return "Đợi thanh toán";
                case OrderStatus.Processing:
                    return "Đang chuẩn bị hàng";
                case OrderStatus.Rejected:
                    return "Bị shop từ chối";
                case OrderStatus.Cancelled:
                    return "Người dùng từ chối";
                case OrderStatus.Prepared:
                    return "Đẫ chuẩn bị đơn hàng";
                case OrderStatus.Delivering:
                    return "Đang giao hàng";
                case OrderStatus.Delivery_Failed:
                    return "Giao hàng thất bại";
                case OrderStatus.Success:
                    return "Giao hàng thành công";
                default:
                    throw new Exception("Unknonw messagae");
            }
        }
    }
}
