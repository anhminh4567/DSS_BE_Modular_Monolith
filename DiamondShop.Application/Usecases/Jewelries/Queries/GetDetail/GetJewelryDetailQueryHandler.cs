using DiamondShop.Commons;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetDetail
{
    public record GetJewelryDetailQuery(string jewelryId) : IRequest<Result<Jewelry>>;
    internal class GetJewelryDetailQueryHandler : IRequestHandler<GetJewelryDetailQuery, Result<Jewelry>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryService _jewelryService;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IDiscountRepository _discountRepository;

        public GetJewelryDetailQueryHandler(IJewelryRepository jewelryRepository, IJewelryService jewelryService, ISizeMetalRepository sizeMetalRepository, IDiscountRepository discountRepository)
        {
            _jewelryRepository = jewelryRepository;
            _jewelryService = jewelryService;
            _sizeMetalRepository = sizeMetalRepository;
            _discountRepository = discountRepository;
        }

        public async Task<Result<Jewelry>> Handle(GetJewelryDetailQuery request, CancellationToken token)
        {
            var query = _jewelryRepository.GetQuery();
            query = _jewelryRepository.QueryInclude(query, p => p.Model);
            query = _jewelryRepository.QueryInclude(query, p => p.Model.Category);
            query = _jewelryRepository.QueryInclude(query, p => p.Size);
            query = _jewelryRepository.QueryInclude(query, p => p.Metal);
            query = _jewelryRepository.QueryInclude(query, p => p.Diamonds);
            query = _jewelryRepository.QueryFilter(query, p => p.Id == JewelryId.Parse(request.jewelryId));
            query = _jewelryRepository.QuerySplit(query);
            query = _jewelryRepository.QueryInclude(query, p => p.Review);
            var item = query.FirstOrDefault();
            if (item == null) return Result.Fail(JewelryErrors.JewelryNotFoundError);
            var activeDiscount = await _discountRepository.GetActiveDiscount();
            var result =  _jewelryService.AddPrice(item, _sizeMetalRepository);
            _jewelryService.AssignJewelryDiscount(result, activeDiscount).Wait();
            return result;
        }
    }
}
