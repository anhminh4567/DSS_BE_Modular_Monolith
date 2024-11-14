using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Queries.GetDetail
{
    public record GetDiscountDetailQuery(string discountId) : IRequest<Result<Discount>>;
    internal class GetDiscountDetailQueryHandler : IRequestHandler<GetDiscountDetailQuery, Result<Discount>>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ILogger<GetDiscountDetailQueryHandler> _logger;

        public GetDiscountDetailQueryHandler(IDiscountRepository discountRepository, ILogger<GetDiscountDetailQueryHandler> logger)
        {
            _discountRepository = discountRepository;
            _logger = logger;
        }

        public async Task<Result<Discount>> Handle(GetDiscountDetailQuery request, CancellationToken cancellationToken)
        {
            var reqId = DiscountId.Parse(request.discountId);
            var result = await _discountRepository.GetById(reqId);
            if (result == null)
                return Result.Fail(new NotFoundError());
            return result;
        }
    }
}
