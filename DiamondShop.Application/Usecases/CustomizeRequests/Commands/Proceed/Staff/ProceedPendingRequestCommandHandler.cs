using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Diamonds.Commands.Create;
using DiamondShop.Application.Usecases.Diamonds.Commands.CreateForCustomizeRequest;
using DiamondShop.Application.Usecases.Jewelries.Commands;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
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
    public record ProceedPendingRequestCommand(CustomizeRequest CustomizeRequest, string? SideDiamondOptId, List<DiamondRequestAssignRecord>? DiamondAssigning) : IRequest<Result<CustomizeRequest>>;
    internal class ProceedPendingRequestCommandHandler : IRequestHandler<ProceedPendingRequestCommand, Result<CustomizeRequest>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IDiamondRequestRepository _diamondRequestRepository;
        private readonly ICustomizeRequestService _customizeRequestService;
        private readonly IDiamondServices _diamondServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;
        public ProceedPendingRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork, ICustomizeRequestService customizeRequestService, IDiamondRepository diamondRepository, IDiamondRequestRepository diamondRequestRepository, IDiamondServices diamondServices, ISideDiamondRepository sideDiamondRepository, ISender sender, IJewelryModelRepository jewelryModelRepository)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
            _customizeRequestService = customizeRequestService;
            _diamondRepository = diamondRepository;
            _diamondRequestRepository = diamondRequestRepository;
            _diamondServices = diamondServices;
            _sideDiamondRepository = sideDiamondRepository;
            _sender = sender;
            _jewelryModelRepository = jewelryModelRepository;
        }

        public async Task<Result<CustomizeRequest>> Handle(ProceedPendingRequestCommand request, CancellationToken token)
        {
            request.Deconstruct(out CustomizeRequest customizeRequest, out string? sideDiamondOptId, out List<DiamondRequestAssignRecord>? diamondAssigning);
            await _unitOfWork.BeginTransactionAsync(token);
            if (customizeRequest.Status != CustomizeRequestStatus.Pending)
                return Result.Fail(CustomizeRequestErrors.UnpricableError);
            var model = await _jewelryModelRepository.GetById(customizeRequest.JewelryModelId);
            if (model == null)
                return Result.Fail(JewelryModelErrors.JewelryModelNotFoundError);
            if (model.SideDiamonds.Count() > 0)
            {
                if (customizeRequest.SideDiamondId != null || sideDiamondOptId != null)
                {
                    var sideDiamond = model.SideDiamonds.FirstOrDefault(p => customizeRequest.SideDiamondId == null ? p.Id.Value == sideDiamondOptId : p.Id == customizeRequest.SideDiamondId);
                    if (sideDiamond == null)
                        return Result.Fail(JewelryModelErrors.SideDiamond.SideDiamondOptNotFoundError);
                    await _diamondServices.GetSideDiamondPrice(sideDiamond);
                    if (sideDiamond.TotalPrice == 0)
                        return Result.Fail(JewelryModelErrors.SideDiamond.UnpricedSideDiamondOptError);
                }
                else
                    return Result.Fail(CustomizeRequestErrors.UnchosenSideDiamondOptError);
            }

            if (customizeRequest.DiamondRequests.Any())
            {
                if (customizeRequest.DiamondRequests.Count != diamondAssigning.Count)
                    return Result.Fail(JewelryModelErrors.MainDiamond.MainDiamondCountError(customizeRequest.DiamondRequests.Count));
                List<IError> errors = new List<IError>();
                var diamondRequests = customizeRequest.DiamondRequests;
                for (int i = 0; i < diamondRequests.Count; i++)
                {
                    var diamondRequest = diamondRequests[i];
                    var assignedDiamond = diamondAssigning.FirstOrDefault(p => p.DiamondRequestId == diamondRequest.DiamondRequestId.Value);

                    if (assignedDiamond.DiamondId == null && assignedDiamond.CreateDiamondCommand == null)
                    {
                        errors.Add(CustomizeRequestErrors.DiamondRequest.UnchosenMainDiamondError(i+1));
                    }
                    else
                    {
                        //Add existing diamond
                        if (assignedDiamond.DiamondId != null)
                        {
                            var diamond = await _diamondRepository.GetById(DiamondId.Parse(assignedDiamond.DiamondId));
                            if (diamond == null)
                            {
                                errors.Add(new Error($"Kim cương số {i}: {DiamondErrors.DiamondNotFoundError.Message}"));
                            }
                            else if ((diamond.Status != ProductStatus.Active && diamond.Status != ProductStatus.PreOrder) || diamond.JewelryId != null)
                            {
                                errors.Add(new Error($"Kim cương số {i}: {DiamondErrors.DiamondNotAvailable.Message}"));
                            }
                            else
                            {
                                var matchingFlag = _customizeRequestService.IsAssigningDiamondSpecValid(diamondRequest, diamond);
                                if (matchingFlag.IsFailed)
                                {
                                    errors.Add(new Error($"Kim cương số {i}: {DiamondErrors.DiamondNotValid().Message}"));
                                }
                                else
                                {
                                    var prices = await _diamondServices.GetPrice(diamond.Cut, diamond.DiamondShape, diamond.IsLabDiamond, token);
                                    var price = await _diamondServices.GetDiamondPrice(diamond, prices);
                                    if (price == null || diamond.IsPriceKnown == false)
                                    {
                                        errors.Add(new Error($"Kim cương số {i}: {DiamondErrors.UnknownPrice.Message}"));
                                    }
                                    //Only when valid
                                    else
                                    {
                                        diamondRequest.DiamondId = diamond.Id;
                                        await _diamondRequestRepository.Update(diamondRequest);
                                        if (diamond.Status != ProductStatus.PreOrder)
                                            diamond.SetLockForCustomizeRequest();
                                        await _diamondRepository.Update(diamond);
                                        await _unitOfWork.SaveChangesAsync(token);
                                    }
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
            customizeRequest.SetPriced();
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return customizeRequest;
        }
    }
}
