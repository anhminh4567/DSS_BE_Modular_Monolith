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
    public record GetAllCustomizeRequestQuery(int CurrentPage = 0, int PageSize = 20, string? Email = null, string? CreatedDate = null, string? ExpiredDate = null, CustomizeRequestStatus? Status = null) : IRequest<Result<PagingResponseDto<CustomizeRequest>>>;
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
            request.Deconstruct(out int currentPage, out int pageSize, out string? email, out string? createdDate, out string? expiredDate, out CustomizeRequestStatus? status);
            var query = _customizeRequestRepository.GetQuery();
            query = _customizeRequestRepository.QueryInclude(query, p => p.Account);
            query = _customizeRequestRepository.QueryInclude(query, p => p.DiamondRequests);
            if (email != null)
                query = _customizeRequestRepository.QueryFilter(query, p => p.Account.Email.ToUpper().Contains(email.ToUpper()));
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
            //TODO: Add filter
            int maxPage = (int)Math.Ceiling((decimal)query.Count() / pageSize);
            var list = query.Skip(currentPage * pageSize).Take(pageSize).ToList();
            return new PagingResponseDto<CustomizeRequest>(maxPage, currentPage + 1, list);
        }
    }
}
