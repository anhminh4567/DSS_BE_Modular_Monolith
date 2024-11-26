using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetAll
{
    public record GetAllCustomizeRequestQuery(int CurrentPage, int PageSize, string? Code, string? Email, DateTime? CreatedDate, DateTime? ExpiredDate, CustomizeRequestStatus? Status) : IRequest<Result<PagingResponseDto<CustomizeRequest>>>;
    internal class GetAllCustomizeRequestQueryHandler : IRequestHandler<GetAllCustomizeRequestQuery, Result<PagingResponseDto<CustomizeRequest>>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IJewelryModelService _jewelryModelService;
        public GetAllCustomizeRequestQueryHandler(ICustomizeRequestRepository customizeRequestRepository)
        {
            _customizeRequestRepository = customizeRequestRepository;
        }

        public async Task<Result<PagingResponseDto<CustomizeRequest>>> Handle(GetAllCustomizeRequestQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int currentPage, out int pageSize, out string? code, out string? email, out DateTime? createdDate, out DateTime? expiredDate, out CustomizeRequestStatus? status);
            currentPage = currentPage == 0 ? 1 : currentPage;
            pageSize = pageSize == 0 ? 20 : pageSize;
            var query = _customizeRequestRepository.GetQuery();
            query = _customizeRequestRepository.QueryInclude(query, p => p.Account);
            query = _customizeRequestRepository.QueryInclude(query, p => p.DiamondRequests);
            if (!String.IsNullOrEmpty(code))
                query = _customizeRequestRepository.QueryFilter(query, p => p.RequestCode.ToUpper().Contains(code.ToUpper()));
            if (!String.IsNullOrEmpty(email))
                query = _customizeRequestRepository.QueryFilter(query, p => p.Account.Email.ToUpper().Contains(email.ToUpper()));
            if (createdDate != null)
            {
                query = _customizeRequestRepository.QueryFilter(query, p => p.CreatedDate == createdDate);
            }
            if (expiredDate != null)
            {
                query = _customizeRequestRepository.QueryFilter(query, p => p.ExpiredDate >= expiredDate);
            }
            if (status != null)
                query = _customizeRequestRepository.QueryFilter(query, p => p.Status == status);
            query = _customizeRequestRepository.QueryOrderBy(query, p => p.OrderByDescending(k => k.CreatedDate));
            int maxPage = (int)Math.Ceiling((decimal)query.Count() / pageSize);
            var list = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            return new PagingResponseDto<CustomizeRequest>(maxPage, currentPage, list);
        }
    }
}
