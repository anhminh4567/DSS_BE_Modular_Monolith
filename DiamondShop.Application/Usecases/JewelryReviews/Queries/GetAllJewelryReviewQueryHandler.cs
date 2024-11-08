using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces.JewelryReviews;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.JewelryReviewRepo;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryReviews.Queries
{
    public record GetAllJewelryReviewQuery(int CurrentPage, int PageSize, string ModelId, string? MetalId, bool OrderByOldest) : IRequest<PagingResponseDto<JewelryReview>>;
    internal class GetAllJewelryReviewQueryHandler : IRequestHandler<GetAllJewelryReviewQuery, PagingResponseDto<JewelryReview>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryReviewRepository _jewelryReviewRepository;
        private readonly IJewelryReviewFileService _jewelryReviewFileService;

        public GetAllJewelryReviewQueryHandler(IJewelryRepository jewelryRepository, IJewelryReviewFileService jewelryReviewFileService, IJewelryReviewRepository jewelryReviewRepository)
        {
            _jewelryRepository = jewelryRepository;
            _jewelryReviewFileService = jewelryReviewFileService;
            _jewelryReviewRepository = jewelryReviewRepository;
        }

        public async Task<PagingResponseDto<JewelryReview>> Handle(GetAllJewelryReviewQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int currentPage, out int pageSize, out string modelId, out string? metalId, out bool orderByOldest);
            currentPage = currentPage == 0 ? 1 : currentPage;
            pageSize = pageSize == 0 ? 20 : pageSize;
            var query = _jewelryReviewRepository.GetQuery();
            query = _jewelryReviewRepository.QueryInclude(query, p => p.Jewelry);
            query = _jewelryReviewRepository.QueryInclude(query, p => p.Account);
            query = _jewelryReviewRepository.QueryFilter(query, p => p.Jewelry.ModelId == JewelryModelId.Parse(modelId));
            if (metalId != null)
                query = _jewelryReviewRepository.QueryFilter(query, p => p.Jewelry.MetalId == MetalId.Parse(metalId));
            var orderedQuery = orderByOldest ? query.OrderBy(p => p.ModifiedDate) : query.OrderByDescending(p => p.ModifiedDate);
            var maxPage = (int)Math.Ceiling(orderedQuery.Count() / (decimal)pageSize);
            var reviews = orderedQuery.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            var medias = await _jewelryReviewFileService.GetFolders(JewelryModelId.Parse(modelId), MetalId.Parse(metalId));
            foreach(var review in reviews)
            {
                review.Medias.AddRange(medias.FindAll(p => p.MediaPath.Contains(review.Jewelry.SerialCode)));
            }
            return new PagingResponseDto<JewelryReview>(maxPage, currentPage, reviews);
        }
    }
}
