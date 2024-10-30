using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Domain.Models.Warranties.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Warranties
{
    public class Warranty : Entity<WarrantyId>
    {
        public WarrantyType Type { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int MonthDuration { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now.Date.ToUniversalTime();
        public decimal Price { get; set; }
        private Warranty() { }
        public static Warranty Create(WarrantyType type, string name, string code, int duration, decimal price, WarrantyId givenId = null)
        {
            return new Warranty()
            {
                Id = givenId is null ? WarrantyId.Create() : givenId,
                Name = name,
                Code = code,
                MonthDuration = duration,
                Price = price,
                Type = type,
            };
        }
    }
}
