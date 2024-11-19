using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Staff;
using DiamondShop.Application.Usecases.Diamonds.Commands.Create;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.CreateForCustomizeRequest
{
    public record CreateDiamondWhenNotExistCommand(CreateDiamondCommand CreateDiamond , string customizeRequestId, string diamondRequestId, decimal? lockPrice   ) : IRequest<Result<Diamond>>;
    internal class CreateDiamondWhenNotExistCommandHandler : IRequestHandler<CreateDiamondWhenNotExistCommand, Result<Diamond>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondServices _diamondServices;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IDiamondRequestRepository _diamondRequestRepository;
        private readonly ICustomizeRequestService _customizeRequestService;
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public CreateDiamondWhenNotExistCommandHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IDiamondServices diamondServices, IDiamondPriceRepository diamondPriceRepository, ICustomizeRequestRepository customizeRequestRepository, IDiamondRequestRepository diamondRequestRepository, ICustomizeRequestService customizeRequestService, ISender sender, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _diamondServices = diamondServices;
            _diamondPriceRepository = diamondPriceRepository;
            _customizeRequestRepository = customizeRequestRepository;
            _diamondRequestRepository = diamondRequestRepository;
            _customizeRequestService = customizeRequestService;
            _sender = sender;
            _mapper = mapper;
        }

        public async Task<Result<Diamond>> Handle(CreateDiamondWhenNotExistCommand request, CancellationToken cancellationToken)
        {
            CustomizeRequestId customizeRequestId = CustomizeRequestId.Parse(request.customizeRequestId);
            DiamondRequestId diamondRequestId = DiamondRequestId.Parse(request.diamondRequestId);
            var getCustomizeRequest = await _customizeRequestRepository.GetById(customizeRequestId);
            if(getCustomizeRequest is null)
                return Result.Fail(new NotFoundError("Không tìm thấy yêu cầu này"));
            await _unitOfWork.BeginTransactionAsync();
            var diamondRequest = getCustomizeRequest.DiamondRequests.FirstOrDefault(x => x.DiamondRequestId == diamondRequestId);
            var createResult = await _sender.Send(request.CreateDiamond);
            if(createResult.IsFailed)
            {
                await _unitOfWork.RollBackAsync();
                return createResult;
            }
            var diamond = createResult.Value;
            diamond.Status = Domain.Common.Enums.ProductStatus.PreOrder;
            
            var isDiamondMetRquirement = _customizeRequestService.IsAssigningDiamondSpecValid(diamondRequest, diamond);
            if(isDiamondMetRquirement is false)
            {
                await _unitOfWork.RollBackAsync();
                return Result.Fail(new ValidationError("Diamond không đáp ứng yêu cầu Requirement"));
            }
            if(request.lockPrice != null)
            {
                var normalizedPrice = MoneyVndRoundUpRules.RoundAmountFromDecimal(request.lockPrice.Value);
                if (request.lockPrice < 0 || normalizedPrice < 0)
                {
                    await _unitOfWork.RollBackAsync();
                    return Result.Fail(new ValidationError("Giá không hợp lệ, Phải lớn hơn hoặc =0"));
                }
            }
            diamondRequest.AssignDiamondToRequest(diamond);
            await _diamondRepository.Update(diamond);
            await _customizeRequestRepository.Update(getCustomizeRequest);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return Result.Ok(diamond);
        }
    }

}
