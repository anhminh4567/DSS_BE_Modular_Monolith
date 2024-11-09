using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Transactions
{
    public class PaymentMethodDto
    {
        public string Id { get; set; }
        public string MethodName { get; set; }
        public string? MethodThumbnailPath { get; set; }
        public bool Status { get; set; }
        public string? MappedName { get; set; }
    }
}
