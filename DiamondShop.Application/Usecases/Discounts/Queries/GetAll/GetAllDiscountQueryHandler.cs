using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Queries.GetAll
{

    public record GetAllDiscountQuery() : IRequest<List<Discount>>;
    internal class GetAllDiscountQueryHandler : IRequestHandler<GetAllDiscountQuery, List<Discount>>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ILogger<GetAllDiscountQueryHandler> _logger;
        public GetAllDiscountQueryHandler(IDiscountRepository discountRepository, ILogger<GetAllDiscountQueryHandler> logger)
        {
            _discountRepository = discountRepository;
            _logger = logger;
        }

        public async Task<List<Discount>> Handle(GetAllDiscountQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("get all discount is called");
            var result = await _discountRepository.GetAll();
            return result;
        }
    }
}
