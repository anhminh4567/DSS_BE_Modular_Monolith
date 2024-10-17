using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Usecases.Jewelries.Queries.GetSelling;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetSelling
{
    public record GetModelDetail(string? Category, MetalId? MetalId, decimal? MinPrice, decimal? MaxPrice, bool? IsRhodiumFinished, bool? IsEngravable, bool? isFinished);
    public record GetSellingJewelryModelQuery(int pageSize = 20, int start = 0, GetModelDetail GetModelDetail = null) : IRequest<Result<PagingResponseDto<JewelryModel>>>;
    internal class GetSellingJewelryModelQueryHandler : IRequestHandler<GetSellingJewelryModelQuery, Result<PagingResponseDto<JewelryModel>>>
    {
        private readonly IJewelryModelCategoryRepository _categoryRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _modelRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly IJewelryService _jewelryService;

        public GetSellingJewelryModelQueryHandler(IJewelryRepository jewelryRepository, ISizeMetalRepository sizeMetalRepository, IJewelryModelRepository modelRepository, IJewelryService jewelryService, IMainDiamondRepository mainDiamondRepository, IJewelryModelCategoryRepository categoryRepository, ISideDiamondRepository sideDiamondRepository)
        {
            _jewelryRepository = jewelryRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _modelRepository = modelRepository;
            _jewelryService = jewelryService;
            _mainDiamondRepository = mainDiamondRepository;
            _categoryRepository = categoryRepository;
            _sideDiamondRepository = sideDiamondRepository;
        }
        public async Task<Result<PagingResponseDto<JewelryModel>>> Handle(GetSellingJewelryModelQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int pageSize, out int start, out GetModelDetail getModelDetail);

            var query = _modelRepository.GetQuery();
            query = _modelRepository.QueryInclude(query, p => p.SideDiamonds);

            var sideQuery = _sideDiamondRepository.GetQuery();
            sideQuery = _sideDiamondRepository.QueryInclude(sideQuery, p => p.Model);

            var test = await _modelRepository.GetSellingModel();
            throw new Exception();
        }
        private PagingResponseDto<JewelryModel> BlankPaging() => new PagingResponseDto<JewelryModel>(
                  totalPage: 0,
                  currentPage: 0,
                  Values: []
                  );
    }
}
