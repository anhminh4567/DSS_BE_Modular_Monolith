using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DeliveryFees.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DeliveryFees
{
    public class DeliveryFee : Entity<DeliveryFeeId>
    {
        public string DeliveryMethod { get; set; } = "Xe Ô tô, do của hàng tự vận chuyển";
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int? FromKm { get; set; }
        public int? ToKm { get; set; }
        public string? FromLocation { get; set; }// cho truong hop ko co api tinh gia thi moi cai location ra
        public string? ToLocation { get; set; }
        [NotMapped]
        public bool IsDistancePriceType { get => FromKm.HasValue && ToKm.HasValue; }
        public static DeliveryFee CreateDistanceType(string name, decimal cost, int fromKm, int toKm)
        {
            return new DeliveryFee
            {
                Id = DeliveryFeeId.Create(),
                Cost = cost,
                Name = name,
                FromKm = fromKm,
                ToKm = toKm,
            };
        }
        public static DeliveryFee CreateLocationType(string name, decimal cost, string fromCity, string toCity)
        {
            return new DeliveryFee
            {
                Id = DeliveryFeeId.Create(),
                Cost = cost,
                Name = name,
                FromLocation = fromCity,
                ToLocation = toCity,
            };
        }
        public void ChangeName( string name) => Name = name;
        public void ChangeCost( decimal cost) => Cost = cost;
        public void ChangeFromToKm(int from, int to)
        {
            FromKm = from; ToKm = to;
            FromLocation = null; ToLocation = null;
        }
        public void ChangeFromToCity(string fromCity, string toCity)
        {
            FromLocation = fromCity; ToLocation = toCity;
            FromKm = null; ToKm = null;
        }

        private DeliveryFee()
        {
        }
    }
}
