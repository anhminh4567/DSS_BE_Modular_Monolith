using DiamondShop.Application.Commons.Responses;
using DiamondShop.Commons;
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
    public record GetJewelryDetail(string? Category, MetalId? MetalId, decimal? MinPrice, decimal? MaxPrice, bool? IsRhodiumFinished, bool? IsEngravable, bool? isFinished);
    public record GetSellingJewelryQuery(int pageSize = 20, int start = 0, GetJewelryDetail GetJewelryDetail = null) : IRequest<Result<PagingResponseDto<Jewelry>>>;
    internal class GetSellingJewelryQueryHandler : IRequestHandler<GetSellingJewelryQuery, Result<PagingResponseDto<Jewelry>>>
    {
        private readonly IJewelryModelCategoryRepository _categoryRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _modelRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly IJewelryService _jewelryService;

        public GetSellingJewelryQueryHandler(IJewelryRepository jewelryRepository, ISizeMetalRepository sizeMetalRepository, IJewelryModelRepository modelRepository, IJewelryService jewelryService, IMainDiamondRepository mainDiamondRepository, IJewelryModelCategoryRepository categoryRepository)
        {
            _jewelryRepository = jewelryRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _modelRepository = modelRepository;
            _jewelryService = jewelryService;
            _mainDiamondRepository = mainDiamondRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<PagingResponseDto<Jewelry>>> Handle(GetSellingJewelryQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int pageSize, out int start, out GetJewelryDetail getJewelryDetail);
            var sizeMetalQuery = _sizeMetalRepository.GetQuery();
            sizeMetalQuery = _sizeMetalRepository.QueryInclude(sizeMetalQuery, p => p.Metal);

            var query = _jewelryRepository.GetQuery();
            query = _jewelryRepository.QueryInclude(query, p => p.Metal); 
            query = _jewelryRepository.QueryInclude(query, p => p.Model);
            query = _jewelryRepository.QueryInclude(query, p => p.Model.MainDiamonds);
            query = _jewelryRepository.QueryInclude(query, p => p.Diamonds);
            query = _jewelryRepository.QueryInclude(query, p => p.SideDiamonds);
            query = _jewelryRepository.QueryFilter(query, p => p.IsActive);
            if (!String.IsNullOrEmpty(getJewelryDetail?.Category))
            {
                var category = await _categoryRepository.ContainsName(getJewelryDetail.Category);
                if (category == null)
                {
                    return BlankPaging();
                }
                query = _jewelryRepository.QueryFilter(query, p => p.Model.CategoryId == category.Id);
            }
            if (getJewelryDetail?.MetalId != null)
            {
                sizeMetalQuery = _sizeMetalRepository.QueryFilter(sizeMetalQuery, p => p.MetalId == getJewelryDetail.MetalId);
                query = _jewelryRepository.QueryFilter(query, p => p.MetalId == getJewelryDetail.MetalId);
            }
            if (getJewelryDetail?.IsRhodiumFinished != null)
            {
                query = _jewelryRepository.QueryFilter(query, p => p.Model.IsRhodiumFinish == getJewelryDetail.IsRhodiumFinished);
            }
            if (getJewelryDetail?.IsEngravable != null)
            {
                query = _jewelryRepository.QueryFilter(query, p => p.Model.IsEngravable == getJewelryDetail.IsEngravable);
            }
            var sizeMetals = sizeMetalQuery.ToList();
            sizeMetals.ForEach(p => { p.Price = (decimal)p.Weight * p.Metal?.Price ?? 0; });
            if (getJewelryDetail?.MinPrice != null)
            {
                sizeMetals = sizeMetals.Where(p => p.Price >= getJewelryDetail.MinPrice).ToList();
            }
            if (getJewelryDetail?.MaxPrice != null)
            {
                sizeMetals = sizeMetals.Where(p => p.Price <= getJewelryDetail.MaxPrice).ToList();
            }
            if (sizeMetals.Count == 0)
                return BlankPaging();
            query = _jewelryRepository.QuerySplit(query);
            var enumerables = query.AsEnumerable().Where(
                p => sizeMetals.Any(k => k.SizeId == p.SizeId && k.MetalId == p.MetalId && k.ModelId == p.ModelId));
            if (getJewelryDetail?.isFinished != null)
            {
                enumerables = enumerables.Where(p =>
                (p.Diamonds.Count() == p.Model.MainDiamonds.Sum(p => p.Quantity)) && (bool)getJewelryDetail.isFinished);
            }
            var count = query.Count();
            query.Skip(start * pageSize);
            query.Take(pageSize);
            var result = enumerables.ToList();
            var totalPage = (int)Math.Ceiling((decimal)count / (decimal)pageSize);
            var addFlag = _jewelryService.SetupUnmapped(result, sizeMetals);
            if (!addFlag)
                return Result.Fail(new ConflictError("Can't get jewelries' price"));
            var response = new PagingResponseDto<Jewelry>(
                totalPage: totalPage,
                currentPage: start + 1,
                Values: result.ToList()
                );
            return response;
        }
        private PagingResponseDto<Jewelry> BlankPaging() => new PagingResponseDto<Jewelry>(
                    totalPage: 0,
                    currentPage: 0,
                    Values: []
                    );
    }
}
