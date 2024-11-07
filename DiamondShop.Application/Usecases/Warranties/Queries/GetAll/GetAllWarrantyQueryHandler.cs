using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.Warranties;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Domain.Repositories;
using MediatR;

namespace DiamondShop.Application.Usecases.Warranties.Queries.GetAll
{
    public record GetAllWarrantyQuery(int CurrentPage, int PageSize, string? Name, string? Code, decimal? MinPrice, decimal? MaxPrice, WarrantyType? WarrantyType) : IRequest<PagingResponseDto<Warranty>>;
    internal class GetAllWarrantyQueryHandler : IRequestHandler<GetAllWarrantyQuery, PagingResponseDto<Warranty>>
    {
        private readonly IWarrantyRepository _warrantyRepository;

        public GetAllWarrantyQueryHandler(IWarrantyRepository warrantyRepository)
        {
            _warrantyRepository = warrantyRepository;
        }

        public async Task<PagingResponseDto<Warranty>> Handle(GetAllWarrantyQuery request, CancellationToken token)
        {
            request.Deconstruct(out int currentPage, out int pageSize, out string? name, out string? code, out decimal? minPrice, out decimal? maxPrice, out WarrantyType? warrantyType);
            currentPage = currentPage == 0 ? 1 : currentPage;
            pageSize = pageSize == 0 ? 20 : pageSize;
            var query = _warrantyRepository.GetQuery();
            if (name != null)
                query = _warrantyRepository.QueryFilter(query, p => p.Name.ToUpper().Contains(name.ToUpper()));
            if (code != null)
                query = _warrantyRepository.QueryFilter(query, p => p.Code.ToUpper().Contains(code.ToUpper()));
            if (warrantyType != null)
                query = _warrantyRepository.QueryFilter(query, p => p.Type == warrantyType);
            if (minPrice != null)
                query = _warrantyRepository.QueryFilter(query, p => p.Price >= minPrice);
            if (maxPrice != null)
                query = _warrantyRepository.QueryFilter(query, p => p.Price <= maxPrice);
            var maxPage = (int)Math.Ceiling(query.Count() / (decimal)pageSize);
            var list = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            return new PagingResponseDto<Warranty>(maxPage, currentPage, list);
        }
    }
}
