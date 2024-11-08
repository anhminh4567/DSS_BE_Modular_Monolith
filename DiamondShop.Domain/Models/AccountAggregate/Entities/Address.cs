using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.AccountAggregate.Entities
{
    public class Address : Entity<AddressId>
    {
        public AccountId AccountId { get; set; }
        public Account? Account { get; set; }    
        public string Province { get; set; }
        public int? ProvinceId { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Street { get; set; }
        public bool IsDefault { get; set; } = false;    

        public Address(string province, string district, string ward, string street, AccountId accountId) : base(AddressId.Create())
        {
            Province = province;
            District = district;
            Ward = ward;
            Street = street;
        }
        public static Address Create(int provinceId,string province, string district, string ward, string street, AccountId accountId, AddressId? addressId = null)
        {
            return new Address()
            {
                ProvinceId = provinceId,
                AccountId = accountId,
                District = district,
                Id = addressId ?? AddressId.Create(),
                Street = street,
                Ward = ward,
                Province=province,
            };
        }
        public void Update(int provinceId,string province, string district, string ward, string street)
        {
            ProvinceId = provinceId;
            Province = province.Trim();
            District = district.Trim();
            Ward = ward.Trim();
            Street = street.Trim();
        }
        public void ChangeDefault(bool setDefault)
        {
            IsDefault = setDefault;
        }
        public override string? ToString()
        {
            return $"{Province}, {District}, {Ward}, {Street}";
        }

        private Address()
        {
        }
    }
}
