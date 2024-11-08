using DiamondShop.Application.Dtos.Requests.Accounts;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Deliveries;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using DiamondShop.Domain.Services.interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records.PivotTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Deliveries
{
    internal class DeliveryFeeServices : IDeliveryFeeServices
    {
        private readonly ILocationService _locationService;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly IDeliveryService _deliveryService;

        public DeliveryFeeServices(ILocationService locationService, IDeliveryFeeRepository deliveryFeeRepository, IDeliveryService deliveryService)
        {
            _locationService = locationService;
            _deliveryFeeRepository = deliveryFeeRepository;
            _deliveryService = deliveryService;
        }

        public async Task<ShippingPrice> GetShippingPrice(AddressRequestDto addressRequestDto)
        {
            var shipPrice = new ShippingPrice();
            var shopLocation = _locationService.GetShopLocation();
            var createShopAddress = Address.Create(0,shopLocation.Province, shopLocation.District, shopLocation.Ward, shopLocation.Road, AccountId.Parse("0"), AddressId.Parse("0"));
            shipPrice.From = createShopAddress;
            var city = addressRequestDto.Province;
            var getCityIfFound = _locationService.GetProvinces().FirstOrDefault(x => x.Name == city);
            if (getCityIfFound == null)
            {
                shipPrice.To = null;
                return shipPrice;
            }
            var createDestinationAddress = Address.Create(0, addressRequestDto.Province, addressRequestDto.District, addressRequestDto.Ward, addressRequestDto.Street, AccountId.Parse("1"), AddressId.Parse("1"));
            shipPrice.To = createDestinationAddress;
            var getDeliveryLocationFees = await _deliveryFeeRepository.GetLocationType();
            var deliveryFee = _deliveryService.GetDeliveryFeeForLocation(shipPrice.To, shipPrice.From, getDeliveryLocationFees);
            shipPrice.DeliveryFeeFounded = deliveryFee;
            if (shipPrice.IsValid)
                shipPrice.DefaultPrice = deliveryFee.Cost;
            else
                shipPrice.DefaultPrice = 0;
            return shipPrice;
        }
    }
}
