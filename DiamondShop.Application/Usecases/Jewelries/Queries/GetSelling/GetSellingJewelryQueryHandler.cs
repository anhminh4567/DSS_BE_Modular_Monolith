using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetSelling
{
    public record GetSellingJewelryQuery(int pageSize = 20, int start = 0) : IRequest<Result<PagingResponseDto<Jewelry>>>;
    internal class GetSellingJewelryQueryHandler : IRequestHandler<GetSellingJewelryQuery, Result<PagingResponseDto<Jewelry>>>
    {
        private readonly IJewelryRepository _jewelryRepository;

        public GetSellingJewelryQueryHandler(IJewelryRepository jewelryRepository)
        {
            _jewelryRepository = jewelryRepository;
        }

        public async Task<Result<PagingResponseDto<Jewelry>>> Handle(GetSellingJewelryQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int pageSize, out int start);
            (IEnumerable<Jewelry> result, int totalPage) = await _jewelryRepository.GetSellingJewelry(pageSize * start, pageSize);
            var response = new PagingResponseDto<Jewelry>(
                totalPage: totalPage,
                currentPage: start + 1,
                Values: result.ToList()
                );
            return response;
        }
    }
}
