using DiamondShop.Application.Usecases.Orders.Commands.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record CheckoutRequestDto(BillingDetail BillingDetail, CreateOrderInfo CreateOrderInfo);
}
