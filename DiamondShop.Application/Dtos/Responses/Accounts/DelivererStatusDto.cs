using DiamondShop.Application.Dtos.Responses.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Accounts
{
    public class DelivererStatusDto
    {
        public AccountDto Account { get; set; }
        public bool IsFree { get => OrderCurrentlyHandle == null; }
        public string? BusyMessage { get => GetMessage(); }
        public OrderDto? OrderCurrentlyHandle { get; set; }
        private string? GetMessage()
        {
            if (IsFree)
            {
                return null;
            }
            return "Đang giao cho đơn hàng với mã " + OrderCurrentlyHandle.OrderCode;
        }
    }
}
