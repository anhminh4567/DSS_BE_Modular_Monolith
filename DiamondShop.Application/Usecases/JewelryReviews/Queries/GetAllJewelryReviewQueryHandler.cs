using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces.JewelryReview;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryRepo;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryReviews.Queries
{
    public record GetAllJewelryReviewQuery(int CurrentPage, int PageSize, string ModelId, string? MetalId, bool OrderByOldest) : IRequest<PagingResponseDto<JewelryReview>>;
    internal class GetAllJewelryReviewQueryHandler : IRequestHandler<GetAllJewelryReviewQuery, PagingResponseDto<JewelryReview>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryReviewFileService _jewelryReviewFileService;

        public GetAllJewelryReviewQueryHandler(IJewelryRepository jewelryRepository, IJewelryReviewFileService jewelryReviewFileService)
        {
            _jewelryRepository = jewelryRepository;
            _jewelryReviewFileService = jewelryReviewFileService;
        }

        public async Task<PagingResponseDto<JewelryReview>> Handle(GetAllJewelryReviewQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int currentPage, out int pageSize, out string modelId, out string? metalId, out bool orderByOldest);
            currentPage = currentPage == 0 ? 1 : currentPage;
            pageSize = pageSize == 0 ? 20 : pageSize;
            var query = _jewelryRepository.GetQuery();
            query = _jewelryRepository.QueryInclude(query, p => p.Review);
            query = _jewelryRepository.QueryInclude(query, p => p.Review.Account);
            query = _jewelryRepository.QueryFilter(query, p => p.ModelId == JewelryModelId.Parse(modelId));
            if (metalId != null)
                query = _jewelryRepository.QueryFilter(query, p => p.MetalId == MetalId.Parse(metalId));
            var reviews = query.Select(p => p.Review);
            reviews = orderByOldest ? reviews.OrderBy(p => p.ModifiedDate) : reviews.OrderByDescending(p => p.ModifiedDate);
            var maxPage = (int)Math.Ceiling(reviews.Count() / (decimal)pageSize);
            return new PagingResponseDto<JewelryReview>(maxPage, currentPage, reviews.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList());
        }
    }
}
