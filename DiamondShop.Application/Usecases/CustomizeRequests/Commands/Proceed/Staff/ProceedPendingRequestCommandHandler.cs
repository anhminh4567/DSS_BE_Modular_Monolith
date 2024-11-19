using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Diamonds.Commands.Create;
using DiamondShop.Application.Usecases.Diamonds.Commands.CreateForCustomizeRequest;
using DiamondShop.Application.Usecases.Jewelries.Commands;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Staff
{
    public record DiamondRequestAssignRecord(string DiamondRequestId, string? DiamondId, CreateDiamondCommand? CreateDiamondCommand);
    public record ProceedPendingRequestCommand(CustomizeRequest CustomizeRequest, List<DiamondRequestAssignRecord>? DiamondAssigning) : IRequest<Result<CustomizeRequest>>;
    internal class ProceedPendingRequestCommandHandler : IRequestHandler<ProceedPendingRequestCommand, Result<CustomizeRequest>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IDiamondRequestRepository _diamondRequestRepository;
        private readonly ICustomizeRequestService _customizeRequestService;
        private readonly IDiamondServices _diamondServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;
        public ProceedPendingRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork, ICustomizeRequestService customizeRequestService, IDiamondRepository diamondRepository, IDiamondRequestRepository diamondRequestRepository, IDiamondServices diamondServices, ISideDiamondRepository sideDiamondRepository, ISender sender)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
            _customizeRequestService = customizeRequestService;
            _diamondRepository = diamondRepository;
            _diamondRequestRepository = diamondRequestRepository;
            _diamondServices = diamondServices;
            _sideDiamondRepository = sideDiamondRepository;
            _sender = sender;
        }

        public async Task<Result<CustomizeRequest>> Handle(ProceedPendingRequestCommand request, CancellationToken token)
        {
            request.Deconstruct(out CustomizeRequest customizeRequest, out List<DiamondRequestAssignRecord>? diamondAssigning);
            await _unitOfWork.BeginTransactionAsync(token);
            if (customizeRequest.Status != CustomizeRequestStatus.Pending)
                return Result.Fail("This request can't be priced anymore");
            if (customizeRequest.SideDiamondId != null)
            {
                var sideDiamond = await _sideDiamondRepository.GetById(customizeRequest.SideDiamondId);
                if (sideDiamond == null)
                    return Result.Fail("Can't find the required side diamond option");
                if (sideDiamond.ModelId != customizeRequest.JewelryModelId)
                    return Result.Fail("The jewelry model doesn't support this side diamond option");
                await _diamondServices.GetSideDiamondPrice(sideDiamond);
                if (sideDiamond.TotalPrice == 0)
                    return Result.Fail("Please update the side diamond price first");
            }
            if (customizeRequest.DiamondRequests.Any())
            {
                if (customizeRequest.DiamondRequests.Count != diamondAssigning.Count)
                    return Result.Fail("The quantity of assigning diamond doesn't match the requesting amount");
                List<IError> errors = new List<IError>();
                var diamondRequests = customizeRequest.DiamondRequests;
                for (int i = 0; i < diamondRequests.Count; i++)
                {
                    var diamondRequest = diamondRequests[i];
                    var assignedDiamond = diamondAssigning.FirstOrDefault(p => p.DiamondRequestId == diamondRequest.DiamondRequestId.Value);

                    if (assignedDiamond.DiamondId == null && assignedDiamond.CreateDiamondCommand == null)
                    {
                        errors.Add(new Error($"Diamond request number {i} doesn't have an assigned diamond nor preorder diamond"));
                    }
                    else
                    {
                        //Add existing diamond
                        if (assignedDiamond.DiamondId != null)
                        {
                            var diamond = await _diamondRepository.GetById(DiamondId.Parse(assignedDiamond.DiamondId));
                            if (diamond == null)
                            {
                                errors.Add(new Error($"Diamond for request number {i} doesn't exist"));
                            }
                            else if ((diamond.Status != ProductStatus.Active && diamond.Status != ProductStatus.PreOrder) || diamond.JewelryId != null)
                            {
                                errors.Add(new Error($"Diamond for request number {i} isn't available for sell"));
                            }
                            else if (!_customizeRequestService.IsAssigningDiamondSpecValid(diamondRequest, diamond))
                            {
                                errors.Add(new Error($"Diamond for request number {i} doesn't match the requirement"));
                            }
                            else
                            {
                                var prices = await _diamondServices.GetPrice(diamond.Cut, diamond.DiamondShape, diamond.IsLabDiamond, token);
                                var price = await _diamondServices.GetDiamondPrice(diamond, prices);
                                if (price == null || diamond.IsPriceKnown == false)
                                {
                                    errors.Add(new Error($"Can't get price for diamond in request number {i}"));
                                }
                                //Only when valid
                                else
                                {
                                    diamondRequest.DiamondId = diamond.Id;
                                    await _diamondRequestRepository.Update(diamondRequest);
                                    if (diamond.Status != ProductStatus.PreOrder)
                                        diamond.SetLock();
                                    await _diamondRepository.Update(diamond);
                                    await _unitOfWork.SaveChangesAsync(token);
                                }
                            }
                        }
                        //Add nonexisting diamond
                        else if (assignedDiamond.CreateDiamondCommand != null)
                        {
                            var createFlag = await _sender.Send(new CreateDiamondWhenNotExistCommand(assignedDiamond.CreateDiamondCommand, customizeRequest.Id.Value, diamondRequest.DiamondRequestId.Value, null));
                            if (createFlag.IsFailed)
                                errors.AddRange(createFlag.Errors);
                        }
                    }
                }
                if (errors.Count > 0)
                    return Result.Fail(errors);
            }
            customizeRequest.Status = CustomizeRequestStatus.Priced;
            _customizeRequestService.SetStage(customizeRequest);
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return customizeRequest;
        }
    }
}
