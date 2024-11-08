using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.Domain
{
    public class DeliveryFeeFixture
    {
        public DeliveryFeeFixture()
        {
            LocationRules = new LocationRules();
            ShopAddress = new Address(LocationRules.OriginalProvince,LocationRules.OrignalDistrict,LocationRules.OrignalWard,LocationRules.OrignalRoad,AccountId.Parse("Shop"));
            //DeliveryFees.Add(DeliveryFee.CreateDistanceType("from 0 - 100", 50_000 , 0 , 100));
            //DeliveryFees.Add(DeliveryFee.CreateDistanceType("from 100 - 200", 10_000, 100, 200));
            DeliveryFees.Add(DeliveryFee.CreateLocationType(FirstDestination, 30_000,  FirstDestination,0));
            DeliveryFees.Add(DeliveryFee.CreateLocationType(SecondDestination, 30_000, SecondDestination, 1));
        }
        public List<DeliveryFee> DeliveryFees { get; private set; } = new();
        private LocationRules LocationRules { get; set; }
        public Address ShopAddress { get; private set; }
        public const string FirstDestination = "Hanoi";
        public const string SecondDestination = "Camau";
        public const string FirstDistanceType = "from 0 - 100";
        public const string SecondDistanceType = "from 100 - 200";
    }
    public class DeliveryServicesTest : IClassFixture<DeliveryFeeFixture>
    {
        public const string ANY = "any";
        public static AccountId FakeAccountId = AccountId.Parse("fakeuser");
        DeliveryFeeFixture DeliveryFeeFixture;

        public DeliveryServicesTest(DeliveryFeeFixture deliveryFeeFixture)
        {
            DeliveryFeeFixture = deliveryFeeFixture;
        }
        [Fact]
        public void GetDeliveryFeeForLocation_Correct_Should_Return_OneFee()
        {
            // Arrange
            var userAddress = new Address(DeliveryFeeFixture.FirstDestination, ANY, ANY, ANY,FakeAccountId);
            var deliveryFeesWithLocation = DeliveryFeeFixture.DeliveryFees; 
            // Act
            var result = new DeliveryService().GetDeliveryFeeForLocation(userAddress, DeliveryFeeFixture.ShopAddress, deliveryFeesWithLocation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(deliveryFeesWithLocation.First(x => x.Name == DeliveryFeeFixture.FirstDestination),result);
        }
        [Fact(Skip = "this method is removed ")]
        public void GetDeliveryFeeForDistance_Correct_Should_Return_OneFee()
        {
            // Arrange
            var userAddress = new Address(DeliveryFeeFixture.FirstDistanceType, ANY, ANY, ANY, FakeAccountId);
            var distance = 99m;
            var deliveryFeesWithLocation = DeliveryFeeFixture.DeliveryFees;
            // Act
            var result = new DeliveryService().GetDeliveryFeeForDistance(distance,deliveryFeesWithLocation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(deliveryFeesWithLocation.First(x => x.Name == DeliveryFeeFixture.FirstDistanceType), result);
        }
        [Fact(Skip = "this method is removed ")]
        public void GetDeliveryFeeForDistance_OffRange_Should_Return_Null()
        {
            // Arrange
            var userAddress = new Address(DeliveryFeeFixture.FirstDistanceType, ANY, ANY, ANY, FakeAccountId);
            var distance = 299m;
            var deliveryFeesWithLocation = DeliveryFeeFixture.DeliveryFees;
            // Act
            var result = new DeliveryService().GetDeliveryFeeForDistance(distance, deliveryFeesWithLocation);
            // Assert
            Assert.Null(result);
        }
    }
}
