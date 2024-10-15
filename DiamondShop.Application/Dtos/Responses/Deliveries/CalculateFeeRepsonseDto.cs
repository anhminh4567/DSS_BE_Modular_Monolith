using DiamondShop.Application.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Deliveries
{
    public class CalculateFeeRepsonseDto
    {
        public LocationDistantData LocationDistantData { get; set; }
        public DeliveryFeeDto DeliveryFee { get; set; }
    }
}
