using DiamondShop.Domain.Models.DeliveryFees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Deliveries
{
    public class DeliveryFeeDto
    {
        public string Id { get;set; }
        public string DeliveryMethod { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        //public int? FromKm { get; set; }
        //public int? ToKm { get; set; }
        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }
        public int? ToLocationId { get; set; }
        public bool IsEnabled { get; set; }

        //public bool IsDistancePriceType { get; set; }
    }
}
