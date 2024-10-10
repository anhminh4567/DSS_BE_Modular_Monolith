using DiamondShop.Application.Services.Data;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetDetail
{
    public record GetDiamondDetail(string diamondId ) : IRequest<Result<Diamond>>;
    internal class GetDiamondDetailQueryHandler : IRequestHandler<GetDiamondDetail, Result<Diamond>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly ILogger<GetDiamondDetail> _logger;
        private readonly IDiamondServices _diamondServices;

        public GetDiamondDetailQueryHandler(IDiamondRepository diamondRepository, IDiamondPriceRepository diamondPriceRepository, ILogger<GetDiamondDetail> logger, IDiamondServices diamondServices)
        {
            _diamondRepository = diamondRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _logger = logger;
            _diamondServices = diamondServices;
        }

        public async Task<Result<Diamond>> Handle(GetDiamondDetail request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("called from getDiamondDetail");
            var parsedId = DiamondId.Parse(request.diamondId);
            var getResult = await _diamondRepository.GetById(parsedId);
            if (getResult == null)
            {
                return Result.Fail(new NotFoundError());
            }
            var prices = await _diamondPriceRepository.GetPriceByShapes(getResult.DiamondShape,cancellationToken);
            var diamondPrice = await _diamondServices.GetDiamondPrice(getResult, prices);
            getResult.DiamondPrice = diamondPrice;
            var testingOnly = await _diamondRepository.GetByIdIncludeDiscountAndPromotion(parsedId);
            return Result.Ok(getResult);
        }
    }
}
