using DiamondShop.Application.Dtos.Requests.Accounts;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.Deliveries
{
    public interface IDeliveryFeeServices
    {
        Task<ShippingPrice> GetShippingPrice(AddressRequestDto addressRequestDto);
    }
}
