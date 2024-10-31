using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetSelling
{
    public record GetSellingJewelryQuery(int pageSize = 20, int start = 0, string? Category = null, string? MetalId = null, decimal? MinPrice = null, decimal? MaxPrice = null, bool? IsRhodiumFinished = null, bool? IsEngravable = null, bool? isFinished = null) : IRequest<Result<PagingResponseDto<JewelryModelSellingDto>>>;
    internal class GetSellingJewelryQueryHandler : IRequestHandler<GetSellingJewelryQuery, Result<PagingResponseDto<JewelryModelSellingDto>>>
    {
        private readonly IJewelryModelCategoryRepository _categoryRepository;
        private readonly IJewelryModelRepository _modelRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly IJewelryModelService _jewelryModelService;

        public GetSellingJewelryQueryHandler(
            ISizeMetalRepository sizeMetalRepository,
            IJewelryModelRepository modelRepository,
            IMainDiamondRepository mainDiamondRepository, IJewelryModelCategoryRepository categoryRepository, IJewelryModelService jewelryModelService)
        {
            _sizeMetalRepository = sizeMetalRepository;
            _modelRepository = modelRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _categoryRepository = categoryRepository;
            _jewelryModelService = jewelryModelService;
        }

        public async Task<Result<PagingResponseDto<JewelryModelSellingDto>>> Handle(GetSellingJewelryQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int pageSize, out int start, out string? Category, out string? MetalId, out decimal? MinPrice, out decimal? MaxPrice, out bool? IsRhodiumFinished, out bool? IsEngravable, out bool? isFinished);
            var sizeMetalQuery = _sizeMetalRepository.GetQuery();
            sizeMetalQuery = _sizeMetalRepository.QueryInclude(sizeMetalQuery, p => p.Metal);

            var query = _modelRepository.GetSellingModelQuery();
            if (!String.IsNullOrEmpty(Category))
            {
                var category = await _categoryRepository.ContainsName(Category);
                if (category == null)
                {
                    return BlankPaging();
                }
                query = _modelRepository.QueryFilter(query, p => p.CategoryId == category.Id);
            }
            if (IsRhodiumFinished != null)
            {
                query = _modelRepository.QueryFilter(query, p => p.IsRhodiumFinish == IsRhodiumFinished);
            }
            if (IsEngravable != null)
            {
                query = _modelRepository.QueryFilter(query, p => p.IsEngravable == IsEngravable);
            }
            var models = query.ToList();
            List<JewelryModelSellingDto> sellingModels = new();
            foreach (var model in models)
            {
                //Get Metal grouping
                var metalGroup = model.SizeMetals.Where(p =>
                {
                    if (MetalId != null)
                        return p.MetalId.Value == MetalId;
                    return true;
                })
                    .GroupBy(p => p.MetalId)
                    .Select(p => p.OrderBy(p => p.Size))
                    .First();
                //Get SideDiamond grouping
                List<List<SideDiamondOpt>> sideDiamondOpts = new List<List<SideDiamondOpt>>();

                //TODO: REplace diamond price with real price getter
                const decimal DiamondPrices = 10m;
                //var optsGroup = model.SideDiamonds
                //    .SelectMany(p => p.SideDiamondOpts,
                //    (req, opt) =>
                //    {
                //        opt.Price = DiamondPrices;
                //        return opt;
                //    })
                //    .GroupBy(
                //    p => p.SideDiamondReqId,
                //    p => p);
                //List<List<SideDiamondOpt>> cartesianOpts = new List<List<SideDiamondOpt>>();
                //foreach (var opts in optsGroup)
                //{
                //}
                ////Get Price grouping
                //foreach (var metal in metalGroup)
                //{
                //    _jewelryModelService.GetSizeMetalPrice(metal);

                //    if (MinPrice != null && metal.Price < MinPrice)
                //        continue;
                //    if (MaxPrice != null && metal.Price > MaxPrice)
                //        continue;
                //}

            }
            if (sellingModels.Count == 0)
                return BlankPaging();
            query = _modelRepository.QuerySplit(query);
            var count = query.Count();
            query.Skip(start * pageSize);
            query.Take(pageSize);
            var result = query.ToList();
            var totalPage = (int)Math.Ceiling((decimal)count / (decimal)pageSize);
            var response = new PagingResponseDto<JewelryModelSellingDto>(
                TotalPage: totalPage,
                CurrentPage: start + 1,
                Values: []
                );
            return response;
        }
        private PagingResponseDto<JewelryModelSellingDto> BlankPaging() => new PagingResponseDto<JewelryModelSellingDto>(
                    TotalPage: 0,
                    CurrentPage: 0,
                    Values: []
                    );
        private void BacktrackingOpt(List<List<SideDiamondOpt>> list, int index, IGrouping<SideDiamondOpt, SideDiamondOpt> opts)
        {
            if (index == opts.Count())
                return;
        }
    }
}
