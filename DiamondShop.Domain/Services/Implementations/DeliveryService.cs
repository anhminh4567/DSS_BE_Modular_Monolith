using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public class DeliveryService : IDeliveryService
    {
        public DeliveryFee? GetDeliveryFeeForLocation(Address userAddress, List<DeliveryFee> deliveryFeesWithLocation)
        {
            var userCity = userAddress.Province;// null ToLocatoin is ignored, since the list pass here is expected to have all ToLocation values
            var correctFee = deliveryFeesWithLocation.FirstOrDefault(x => x.ToLocation.ToUpper() == userCity.ToUpper());
            return correctFee;
        }
    }
}
