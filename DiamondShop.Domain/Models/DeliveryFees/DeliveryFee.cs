using DiamondShop.Domain.BusinessRules;
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
        public const string UNKNONW_DELIVERY_ID = "-1";
        public string DeliveryMethod { get; set; } = "Xe Ô tô, do của hàng tự vận chuyển";
        public string Name { get; set; }
        public decimal Cost { get; set; }
        //public int? FromKm { get; set; }
        //public int? ToKm { get; set; }
       // public string? FromLocation { get; set; }// cho truong hop ko co api tinh gia thi moi cai location ra
        public string? ToLocation { get; set; }
        public int? ToLocationId { get; set; }
        public bool IsEnabled { get; set; } = true;
        public static DeliveryFee CreateLocationType(string name, decimal cost, string toCity, int toCityId)//, string fromCity
        {
            return new DeliveryFee
            {
                Id = DeliveryFeeId.Create(),
                Cost = cost,
                Name = name,
                //FromLocation = fromCity,
                ToLocation = toCity,
                ToLocationId = toCityId,
            };
        }
        public static DeliveryFee CreateUnknownDelivery()
        {
            return new DeliveryFee
            {
                Id = DeliveryFeeId.Parse(UNKNONW_DELIVERY_ID),
                Cost = 0,
                Name = "lấy hàng tại shop, xin vui lòng tới lấy hàng sớm nhất có thể",
                //FromLocation = null,
                ToLocation = null,
            };
        }
        public static DeliveryFee CreateSelfTakenFromShopDeliveryFee()
        {
            return new DeliveryFee
            {
                Id = DeliveryFeeId.Parse("-2"),
                Cost = 0,
                Name = "Vui lòng nhập đúng địa chỉ",
                ToLocation = "tại shop",
                DeliveryMethod = "Tự lấy hàng tại shop",
                ToLocationId = -2,
                IsEnabled = true,
            };
        }
        public void ChangeName( string name) => Name = name;
        public void ChangeCost( decimal cost) => Cost = MoneyVndRoundUpRules.RoundAmountFromDecimal(cost);
        //public void ChangeFromToKm(int from, int to)
        //{
        //    FromKm = from; ToKm = to;
        //    FromLocation = null; ToLocation = null;
        //}
        public void ChangeFromToCity(string toCity)//string fromCity
        {
             ToLocation = toCity;//FromLocation = fromCity;
            //FromKm = null; ToKm = null;
        }
        public void SetEnable(bool enable) => IsEnabled = enable;
        public void SetStatus() => IsEnabled = !IsEnabled;
        private DeliveryFee()
        {
        }
    }
}
