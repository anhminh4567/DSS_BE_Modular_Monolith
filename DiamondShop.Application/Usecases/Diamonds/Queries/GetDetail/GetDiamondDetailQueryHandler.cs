using DiamondShop.Application.Services.Data;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
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

        public GetDiamondDetailQueryHandler(IDiamondRepository diamondRepository, IDiamondPriceRepository diamondPriceRepository, ILogger<GetDiamondDetail> logger)
        {
            _diamondRepository = diamondRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _logger = logger;
        }

        public async Task<Result<Diamond>> Handle(GetDiamondDetail request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("called from getDiamondDetail");
            var getResult = await _diamondRepository.GetById(DiamondId.Parse(request.diamondId));
            if (getResult == null)
            {
                return Result.Fail(new NotFoundError());
            }
            return Result.Ok(getResult);
        }
    }
}
