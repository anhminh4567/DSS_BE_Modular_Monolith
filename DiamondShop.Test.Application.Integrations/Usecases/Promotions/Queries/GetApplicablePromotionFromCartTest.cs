using DiamondShop.Application.Dtos.Requests.Accounts;
using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Usecases.Promotions.Queries.GetApplicablePromotions;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;

namespace DiamondShop.Test.Application.Integrations.Usecases.Promotions.Queries
{
    public class GetApplicablePromotionFromCartTest : BaseTest
    {
        public GetApplicablePromotionFromCartTest(InitializeIntegration factory) : base(factory)
        {
        }

        [Fact]
        public async Task Handle_ShouldReturnApplicablePromotions_WhenPromotionsExist()
        {
            // Arrange
            var diamondRepo = _scope.ServiceProvider.GetRequiredService<IDiamondRepository>();
            var jewelryRep = _scope.ServiceProvider.GetRequiredService<IJewelryRepository>();
            var promoRepo = _scope.ServiceProvider.GetRequiredService<IPromotionRepository>();

            var getAllDiamonds = await diamondRepo.GetAll();
            var getAllJewelry = await jewelryRep.GetAll();
            var getAllPromotions = await promoRepo.GetActivePromotion();
            var cartRequestDto = new CartRequestDto
            {
                Items = new List<CartItemRequestDto>
                {
                    new CartItemRequestDto { Id = "1", DiamondId = "9a282eae-41b9-4819-8e26-47760aa4c33c" },
                    new CartItemRequestDto { Id = "2", DiamondId = "dd36d821-324c-48d4-b3ff-93e110afbba7" },
                    new CartItemRequestDto { Id = "3", JewelryId = "4eec5a88-feaa-41c9-8313-ee12899eaa96" }
                },
                UserAddress = new AddressRequestDto()
            };
            var query = new GetApplicablePromotionForCartQuery(cartRequestDto);
            // Act
            var result = await _sender.Send(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal( getAllPromotions.Count,result.Value.Promotions.Count);
        }
    }
}
