using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetSelling
{
    public record GetJewelryDetail(string? Category, MetalId? MetalId, decimal? MinPrice, decimal? MaxPrice, bool? IsRhodiumFinished, bool? IsEngravable);
    public record GetSellingJewelryQuery(int pageSize = 20, int start = 0, GetJewelryDetail? GetJewelryDetail = null) : IRequest<Result<PagingResponseDto<Jewelry>>>;
    internal class GetSellingJewelryQueryHandler : IRequestHandler<GetSellingJewelryQuery, Result<PagingResponseDto<Jewelry>>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _modelRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IJewelryService _jewelryService;

        public GetSellingJewelryQueryHandler(IJewelryRepository jewelryRepository, ISizeMetalRepository sizeMetalRepository, IJewelryModelRepository modelRepository, IJewelryService jewelryService)
        {
            _jewelryRepository = jewelryRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _modelRepository = modelRepository;
            _jewelryService = jewelryService;
        }

        public async Task<Result<PagingResponseDto<Jewelry>>> Handle(GetSellingJewelryQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int pageSize, out int start, out GetJewelryDetail getJewelryDetail);
            var query = _jewelryRepository.GetQuery();
            var modelQuery = _modelRepository.GetQuery();
            var sizeMetalQuery = _sizeMetalRepository.GetQuery();
            query = _jewelryRepository.QueryInclude(query, p => p.Model);
            query = _jewelryRepository.QueryFilter(query, p => p.IsActive);
            if (getJewelryDetail != null)
            {
                modelQuery = _modelRepository.QueryInclude(modelQuery, p => p.Category);
                modelQuery = _modelRepository.QueryFilter(modelQuery, p => p.Category.Name.Equals(getJewelryDetail.Category));
                //var sizeMetalQuery = _sizeMetalRepository.GetQuery();
                //sizeMetalQuery = _sizeMetalRepository.QueryInclude(sizeMetalQuery, p => p.Model.)
            }
            var modelIds = modelQuery.Select(p => p.Id).ToList();
            query = FilteringJewelryModel(query, modelIds);
            var count = query.Count();
            query.Skip(start * pageSize);
            query.Take(pageSize);
            var result = query.ToList();
            var totalPage = (int)Math.Ceiling((decimal)count / (decimal)pageSize);
            
            sizeMetalQuery = _sizeMetalRepository.QueryInclude(sizeMetalQuery, p => p.Metal);
            sizeMetalQuery = _sizeMetalRepository.QueryFilter(sizeMetalQuery, p => modelIds.Contains(p.ModelId));
            var sizeMetals = sizeMetalQuery.ToList();

            _jewelryService.AddPrice(result, sizeMetals);
            // (IEnumerable<Jewelry> result, int totalPage) = await _jewelryRepository.GetSellingJewelry(pageSize * start, pageSize);
            var response = new PagingResponseDto<Jewelry>(
                totalPage: totalPage,
                currentPage: start + 1,
                Values: result.ToList()
                );
            return response;
        }
        private IQueryable<Jewelry> FilteringJewelryModel(IQueryable<Jewelry> query, List<JewelryModelId> modelIds)
        {
            query = _jewelryRepository.QueryFilter(query, p => modelIds.Contains(p.ModelId));
            return query;
        }

    }
}
