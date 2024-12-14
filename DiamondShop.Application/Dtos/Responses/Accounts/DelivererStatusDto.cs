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
        public bool IsFree { get; set; }
        public string? BusyMessage { get; set; }
        public OrderDto? OrderCurrentlyHandle { get; set; }
    }
}
