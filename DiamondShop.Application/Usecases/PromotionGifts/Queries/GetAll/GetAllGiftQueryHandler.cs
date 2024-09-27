using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.PromotionGifts.Queries.GetAll
{
    public record GetAllGiftQuery() : IRequest<List<Gift>>;

    internal class GetAllGiftQueryHandler : IRequestHandler<GetAllGiftQuery, List<Gift>>
    {
        private readonly IGiftRepository _giftRepository;
        private readonly ILogger<GetAllGiftQueryHandler> _logger;

        public GetAllGiftQueryHandler(IGiftRepository giftRepository, ILogger<GetAllGiftQueryHandler> logger)
        {
            _giftRepository = giftRepository;
            _logger = logger;
        }

        public async Task<List<Gift>> Handle(GetAllGiftQuery request, CancellationToken cancellationToken)
        {
            var query = _giftRepository.GetQuery();
            return (query.ToList());
        }
    }
}
