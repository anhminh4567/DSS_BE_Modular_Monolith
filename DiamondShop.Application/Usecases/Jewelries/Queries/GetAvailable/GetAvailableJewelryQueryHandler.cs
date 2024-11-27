using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetAvailable
{
    public record GetAvailableJewelryQuery(string ModelId, string MetalId, string SizeId, string? SideDiamondOptId) : IRequest<Result<string>>;
    internal class GetAvailableJewelryQueryHandler : IRequestHandler<GetAvailableJewelryQuery, Result<string>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;
        public GetAvailableJewelryQueryHandler(
            IJewelryRepository jewelryRepository
, ISideDiamondRepository sideDiamondRepository)
        {
            _jewelryRepository = jewelryRepository;
            _sideDiamondRepository = sideDiamondRepository;
        }

        public async Task<Result<string>> Handle(GetAvailableJewelryQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string modelId, out string metalId, out string sizeId, out string? sideDiamondOptId);
            var query = _jewelryRepository.GetQuery();
            query = _jewelryRepository.QueryFilter(query, p => p.Status == ProductStatus.Active);
            query = _jewelryRepository.QueryFilter(query, p => p.ModelId == JewelryModelId.Parse(modelId));
            query = _jewelryRepository.QueryFilter(query, p => p.MetalId == MetalId.Parse(metalId));
            query = _jewelryRepository.QueryFilter(query, p => p.SizeId == SizeId.Parse(sizeId));
            if (!String.IsNullOrEmpty(sideDiamondOptId))
            {
                var sideDiamondOpt = await _sideDiamondRepository.GetById(SideDiamondOptId.Parse(sideDiamondOptId));
                query = _jewelryRepository.QueryFilter(query, p => p.SideDiamond.ColorMin == sideDiamondOpt.ColorMin && p.SideDiamond.ColorMax == sideDiamondOpt.ColorMax &&
                p.SideDiamond.ClarityMin == sideDiamondOpt.ClarityMin && p.SideDiamond.ClarityMax == sideDiamondOpt.ClarityMax &&
                p.SideDiamond.SettingType == sideDiamondOpt.SettingType && p.SideDiamond.Carat == sideDiamondOpt.CaratWeight && p.SideDiamond.Quantity == sideDiamondOpt.Quantity && p.SideDiamond.DiamondShapeId == sideDiamondOpt.ShapeId
                );
            }
            var jewelry = query.FirstOrDefault();
            if(jewelry == null)
                return Result.Fail(JewelryErrors.JewelryNotFoundError);
            return jewelry.Id.Value;
        }
    }
}
