using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetCustomer
{
    public record GetCustomerRequestDto(int CurrentPage, int PageSize, string? CreatedDate, string? ExpiredDate, CustomizeRequestStatus? Status);
    public record GetCustomerCustomizeRequestQuery(string AccountId, GetCustomerRequestDto GetCustomerRequestDto) : IRequest<PagingResponseDto<CustomizeRequest>>;
    internal class GetCustomerCustomizeRequestQueryHandler : IRequestHandler<GetCustomerCustomizeRequestQuery, PagingResponseDto<CustomizeRequest>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        public GetCustomerCustomizeRequestQueryHandler(ICustomizeRequestRepository customizeRequestRepository)
        {
            _customizeRequestRepository = customizeRequestRepository;
        }

        public async Task<PagingResponseDto<CustomizeRequest>> Handle(GetCustomerCustomizeRequestQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string accountId, out GetCustomerRequestDto getCustomerRequestDto);
            getCustomerRequestDto.Deconstruct(out int currentPage, out int pageSize, out string? createdDate, out string? expiredDate, out CustomizeRequestStatus? status);
            currentPage = currentPage == 0 ? 1 : currentPage;
            pageSize = pageSize == 0 ? 20 : pageSize;
            var query = _customizeRequestRepository.GetQuery();
            query = _customizeRequestRepository.QueryInclude(query, p => p.Account);
            query = _customizeRequestRepository.QueryInclude(query, p => p.DiamondRequests);
            query = _customizeRequestRepository.QueryFilter(query, p => p.AccountId == AccountId.Parse(accountId));
            if (createdDate != null)
            {
                DateTime createdDateParsed = DateTime.ParseExact(createdDate, DateTimeFormatingRules.DateTimeFormat, null);
                query = _customizeRequestRepository.QueryFilter(query, p => p.CreatedDate == createdDateParsed);
            }
            if (expiredDate != null)
            {
                DateTime expiredDateParsed = DateTime.ParseExact(expiredDate, DateTimeFormatingRules.DateTimeFormat, null);
                query = _customizeRequestRepository.QueryFilter(query, p => p.CreatedDate == expiredDateParsed);
            }
            if (status != null)
                query = _customizeRequestRepository.QueryFilter(query, p => p.Status == status);
            int maxPage = (int)Math.Ceiling((decimal)query.Count() / pageSize);
            var orderedQuery = _customizeRequestRepository.QueryOrderBy(query, p => p.OrderBy(k => k.CreatedDate));
            var list = orderedQuery.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            return new PagingResponseDto<CustomizeRequest>(maxPage, currentPage, list);
        }
    }
}
