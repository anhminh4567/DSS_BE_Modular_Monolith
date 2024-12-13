using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetAll
{
    public record GetAllJewelryQuery(int CurrentPage, int PageSize, string JewelryModelId, string? SerialCode, string? MetalId, string? SizeId, ProductStatus? Status) : IRequest<PagingResponseDto<Jewelry>>;
    internal class GetAllJewelryQueryHandler : IRequestHandler<GetAllJewelryQuery, PagingResponseDto<Jewelry>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryService _jewelryService;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IDiscountRepository _discountRepository;

        public GetAllJewelryQueryHandler(IJewelryRepository jewelryRepository, IJewelryService jewelryService, ISizeMetalRepository sizeMetalRepository, IDiscountRepository discountRepository)
        {
            _jewelryRepository = jewelryRepository;
            _jewelryService = jewelryService;
            _sizeMetalRepository = sizeMetalRepository;
            _discountRepository = discountRepository;
        }

        public async Task<PagingResponseDto<Jewelry>> Handle(GetAllJewelryQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int currentPage, out int pageSize, out string jewelryModelId, out string? serialCode, out string? metalId, out string? sizeId, out ProductStatus? status);
            var activeDiscount = await _discountRepository.GetActiveDiscount();
            pageSize = pageSize == 0 ? JewelryRule.MinimumItemPerPaging : pageSize;
            currentPage = currentPage == 0 ? 1 : currentPage;
            var query = _jewelryRepository.GetQuery();
            query = _jewelryRepository.QueryInclude(query, p => p.Model);
            query = _jewelryRepository.QueryInclude(query, p => p.Metal);
            query = _jewelryRepository.QueryInclude(query, p => p.Size);
            query = _jewelryRepository.QueryInclude(query, p => p.Diamonds);
            query = _jewelryRepository.QueryFilter(query, p => p.Model.Id == JewelryModelId.Parse(jewelryModelId));
            if (!string.IsNullOrEmpty(serialCode))
            {
                query = _jewelryRepository.QueryFilter(query, p => p.SerialCode.Contains(serialCode));
            }
            if (!string.IsNullOrEmpty(metalId))
            {
                query = _jewelryRepository.QueryFilter(query, p => p.MetalId == MetalId.Parse(metalId));
            }
            if (!string.IsNullOrEmpty(sizeId))
            {
                query = _jewelryRepository.QueryFilter(query, p => p.SizeId == SizeId.Parse(sizeId));
            }
            if (status != null)
            {
                query = _jewelryRepository.QueryFilter(query, p => p.Status == status);
            }
            else
                query = _jewelryRepository.QueryOrderBy(query, p => p.OrderBy(p => p.Status));
            query = _jewelryRepository.QuerySplit(query);
            int maxPage = (int)Math.Ceiling((decimal)query.Count() / pageSize);
            var list = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            foreach (var item in list)
            {
                _jewelryService.AddPrice(item, _sizeMetalRepository);
                _ = _jewelryService.AssignJewelryDiscount(item, activeDiscount).Result;
            }
            return new PagingResponseDto<Jewelry>(maxPage, currentPage, list);
        }
    }
}
