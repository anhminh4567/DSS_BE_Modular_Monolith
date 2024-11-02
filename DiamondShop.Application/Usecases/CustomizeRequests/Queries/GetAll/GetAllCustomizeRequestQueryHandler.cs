using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetAll
{
    public record GetAllCustomizeRequestQuery(int page = 0, int take = 20) : IRequest<Result<PagingResponseDto<CustomizeRequest>>>;
    internal class GetAllCustomizeRequestQueryHandler : IRequestHandler<GetAllCustomizeRequestQuery, Result<PagingResponseDto<CustomizeRequest>>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;

        public GetAllCustomizeRequestQueryHandler(ICustomizeRequestRepository customizeRequestRepository)
        {
            _customizeRequestRepository = customizeRequestRepository;
        }

        public async Task<Result<PagingResponseDto<CustomizeRequest>>> Handle(GetAllCustomizeRequestQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int page, out int take);
            var query = _customizeRequestRepository.GetQuery();
            //TODO: Add filter
            int maxPage = (int)Math.Ceiling((decimal)query.Count() / take);
            var list = query.Skip(page * take).Take(take).ToList();
            return new PagingResponseDto<CustomizeRequest>(maxPage, page, list);
        }
    }
}
