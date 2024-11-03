using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Staff
{
    public record ProceedRequestingRequestCommand(CustomizeRequest CustomizeRequest) : IRequest<Result<CustomizeRequest>>;
    internal class ProceedRequestingRequestCommandHandler : IRequestHandler<ProceedRequestingRequestCommand, Result<CustomizeRequest>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProceedRequestingRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CustomizeRequest>> Handle(ProceedRequestingRequestCommand request, CancellationToken token)
        {

            request.Deconstruct(out CustomizeRequest customizeRequest);
            await _unitOfWork.BeginTransactionAsync(token);
            if (customizeRequest.Status != CustomizeRequestStatus.Requesting)
                return Result.Fail("This request can't be accepted anymore");
            customizeRequest.Status = CustomizeRequestStatus.Accepted;
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return customizeRequest;
        }
    }
}
