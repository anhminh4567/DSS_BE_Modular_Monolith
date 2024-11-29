using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record RefundConfirmRequestDto(string OrderId, decimal Amount, string TransactionCode, IFormFile Evidence);
}
