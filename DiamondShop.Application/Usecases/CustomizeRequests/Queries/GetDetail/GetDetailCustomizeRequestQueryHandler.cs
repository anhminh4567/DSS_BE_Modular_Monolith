using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetDetail
{
    public record GetDetailCustomizeRequestQuery(string requestId) : IRequest<Result<CustomizeRequest>>;
    internal class GetDetailCustomizeRequestQueryHandler : IRequestHandler<GetDetailCustomizeRequestQuery, Result<CustomizeRequest>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;

        public GetDetailCustomizeRequestQueryHandler(ICustomizeRequestRepository customizeRequestRepository)
        {
            _customizeRequestRepository = customizeRequestRepository;
        }

        public async Task<Result<CustomizeRequest>> Handle(GetDetailCustomizeRequestQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string requestId);
            var customizeRequest = await _customizeRequestRepository.GetById(CustomizeRequestId.Parse(requestId));
            if (customizeRequest == null)
                return Result.Fail("This customize request doesn't exist");
            return customizeRequest;
        }
    }
}
