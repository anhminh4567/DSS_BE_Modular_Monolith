using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Dtos.Responses;
using DiamondShop.Application.Dtos.Responses.Transactions;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.AccountRoleAggregate.ValueObjects;
using DiamondShop.Domain.Models.Blogs.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.DeliveryFees.ValueObjects;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using DiamondShop.Domain.Models.Warranties.ValueObjects;
using Mapster;
using Microsoft.Extensions.Options;

namespace DiamondShop.Application.Mappers
{
    public class GeneralTypeMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AccountId , string>()
                .MapWith(src => src.Value);
            config.NewConfig<AccountRoleId, string>()
                .MapWith(src => src.Value);

            config.NewConfig<DiamondId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<DiamondCriteriaId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<DiamondShapeId, string>()
               .MapWith(src => src.Value);
           
            config.NewConfig<PromotionId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<PromoReqId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<GiftId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<DiscountId, string>()
                   .MapWith(src => src.Value);

            config.NewConfig<JewelryId, string>()
               .MapWith(src => src.Value).Compile();
            config.NewConfig<JewelrySideDiamondId, string>()
               .MapWith(src => src.Value).Compile();

            config.NewConfig<JewelryModelId, string>()
               .MapWith(src => src.Value).Compile();
            config.NewConfig<JewelryModelCategoryId, string>()
                .MapWith(src => src.Value).Compile();
            config.NewConfig<MainDiamondReqId, string>()
                .MapWith(src => src.Value).Compile();
            config.NewConfig<SideDiamondOptId, string>()
                .MapWith(src => src.Value).Compile();

            config.NewConfig<SizeId, string>()
               .MapWith(src => src.Value).Compile();

            config.NewConfig<MetalId, string>()
               .MapWith(src => src.Value).Compile();

            config.NewConfig<BlogId, string>()
                .MapWith(src => src.Value).Compile();

            config.NewConfig<CustomizeRequestId, string>()
                .MapWith(src => src.Value).Compile();
            config.NewConfig<DiamondRequestId, string>()
               .MapWith(src => src.Value).Compile();

            config.NewConfig<OrderId, string>()
                .MapWith(src => src.Value).Compile();
            config.NewConfig<OrderItemId, string>()
                .MapWith(src => src.Value).Compile();
            config.NewConfig<OrderLogId, string>()
                .MapWith(src => src.Value).Compile();
            config.NewConfig<DeliveryFeeId, string>()
                .MapWith(src => src.Value).Compile();
            config.NewConfig<TransactionId, string>()
                .MapWith(src => src.Value).Compile();
            config.NewConfig<PaymentMethodId, string>()
                .MapWith(src => src.Value).Compile();

            config.NewConfig<DeliveryFeeId, string>()
                .MapWith(src => src.Value).Compile();
            config.NewConfig<WarrantyId,string>()
                .MapWith(src => src.Value).Compile();

            config.NewConfig<PaymentMethodId, string>()
                .MapWith(src => src.Value).Compile();
            config.NewConfig<PaymentMethod, PaymentMethodDto>()
                .Map(dest => dest.MappedName, src => PaymentMethodHelper.GetMethodName(src));
        }
    }
}
