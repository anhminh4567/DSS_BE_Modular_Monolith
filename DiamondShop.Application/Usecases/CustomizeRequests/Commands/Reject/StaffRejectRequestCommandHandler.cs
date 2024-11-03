using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Customer;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Reject
{
    public record StaffRejectRequestCommand(string CustomizeRequestId) : IRequest<Result<CustomizeRequest>>;
    internal class StaffRejectRequestCommandHandler : IRequestHandler<StaffRejectRequestCommand, Result<CustomizeRequest>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        public StaffRejectRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CustomizeRequest>> Handle(StaffRejectRequestCommand request, CancellationToken token)
        {

            request.Deconstruct(out string customizeRequestId);
            await _unitOfWork.BeginTransactionAsync(token);
            var customizeRequest = await _customizeRequestRepository.GetById(CustomizeRequestId.Parse(customizeRequestId));
            if (customizeRequest == null)
                return Result.Fail("This request doens't exist");
            if (customizeRequest.Status != CustomizeRequestStatus.Pending && customizeRequest.Status != CustomizeRequestStatus.Requesting)
                return Result.Fail("You can't reject this request anymore");
            customizeRequest.Status = CustomizeRequestStatus.Shop_Rejected;
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return customizeRequest;
        }
    }
}
