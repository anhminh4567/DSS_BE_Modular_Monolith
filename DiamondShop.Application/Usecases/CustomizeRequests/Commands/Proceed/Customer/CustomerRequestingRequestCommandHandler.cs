using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Customer
{
    public record CustomerRequestingRequestCommand(string CustomizeRequestId, string AccountId) : IRequest<Result<CustomizeRequest>>;
    internal class CustomerRequestingRequestCommandHandler : IRequestHandler<CustomerRequestingRequestCommand, Result<CustomizeRequest>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerRequestingRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CustomizeRequest>> Handle(CustomerRequestingRequestCommand request, CancellationToken token)
        {

            request.Deconstruct(out string customizeRequestId, out string accountId);
            await _unitOfWork.BeginTransactionAsync(token);
            var customizeRequest = await _customizeRequestRepository.GetById(CustomizeRequestId.Parse(customizeRequestId));
            if (customizeRequest == null)
                return Result.Fail("This request doens't exist");
            if (customizeRequest.ExpiredDate < DateTime.UtcNow)
            {
                return Result.Fail("This customize request has already been expired");
            }
            if (customizeRequest.AccountId.Value != accountId)
                return Result.Fail("Only the owner of this request can change status to Requesting");
            if (customizeRequest.Status != CustomizeRequestStatus.Priced)
                return Result.Fail("This request can't be requesting anymore");
            customizeRequest.Status = CustomizeRequestStatus.Requesting;
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return customizeRequest;
        }
    }
}
