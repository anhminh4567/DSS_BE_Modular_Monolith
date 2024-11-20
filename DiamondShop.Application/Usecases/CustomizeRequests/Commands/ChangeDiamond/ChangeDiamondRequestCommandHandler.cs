using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Diamonds.Commands.Create;
using DiamondShop.Application.Usecases.Diamonds.Commands.CreateForCustomizeRequest;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ErrorMessages;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using static DiamondShop.Application.Commons.Utilities.FileUltilities;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.ChangeDiamond
{
    public record ChangeDiamondRequestCommand(string CustomizeRequestId, string DiamondRequestId, string? DiamondId, CreateDiamondCommand? CreateDiamondCommand) : IRequest<Result<DiamondRequest>>;
    internal class ChangeDiamondRequestCommandHandler : IRequestHandler<ChangeDiamondRequestCommand, Result<DiamondRequest>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IDiamondRequestRepository _diamondRequestRepository;
        private readonly ICustomizeRequestService _customizeRequestService;
        private readonly IDiamondServices _diamondServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;

        public ChangeDiamondRequestCommandHandler(IDiamondRepository diamondRepository, IDiamondRequestRepository diamondRequestRepository, ICustomizeRequestService customizeRequestService, IDiamondServices diamondServices, IUnitOfWork unitOfWork, ISender sender, ICustomizeRequestRepository customizeRequestRepository)
        {
            _diamondRepository = diamondRepository;
            _diamondRequestRepository = diamondRequestRepository;
            _customizeRequestService = customizeRequestService;
            _diamondServices = diamondServices;
            _unitOfWork = unitOfWork;
            _sender = sender;
            _customizeRequestRepository = customizeRequestRepository;
        }

        public async Task<Result<DiamondRequest>> Handle(ChangeDiamondRequestCommand request, CancellationToken token)
        {
            request.Deconstruct(out string customizeRequestId, out string diamondRequestId, out string? diamondId, out CreateDiamondCommand? createDiamondCommand);
            await _unitOfWork.BeginTransactionAsync(token);
            var customizeRequest = await _customizeRequestRepository.GetById(CustomizeRequestId.Parse(customizeRequestId));
            if (customizeRequest == null)
                return Result.Fail(CustomizeRequestErrors.CustomizeRequestNotFoundError);
            if (customizeRequest.Status != CustomizeRequestStatus.Priced)
                return Result.Fail(CustomizeRequestErrors.DiamondRequest.InvalidChangingDiamondStatusError);
            var diamondRequest = await _diamondRequestRepository.GetById(DiamondRequestId.Parse(diamondRequestId));
            if (diamondRequest == null)
                return Result.Fail(CustomizeRequestErrors.DiamondRequest.DiamondRequestNotFoundError);
            if (diamondRequest.DiamondId != null)
            {

                var oldDiamond = diamondRequest.Diamond;
                if (oldDiamond != null)
                {
                    //Preorder then delete
                    if (oldDiamond.Status == ProductStatus.PreOrder)
                    {
                        diamondRequest.DiamondId = null;
                        _diamondRequestRepository.Update(diamondRequest).Wait();
                        await _unitOfWork.SaveChangesAsync(token);
                        await _diamondRepository.Delete(oldDiamond);
                    }
                    //set selling
                    else
                    {
                        oldDiamond.SetSell();
                        await _diamondRepository.Update(oldDiamond);
                    }
                    await _unitOfWork.SaveChangesAsync(token);
                }
                else
                    return Result.Fail(CustomizeRequestErrors.DiamondRequest.OldAttachedDiamondNotFoundError);
            }
            if (createDiamondCommand != null)
            {
                var createFlag = await _sender.Send(new CreateDiamondWhenNotExistCommand(createDiamondCommand, diamondRequest.CustomizeRequestId.Value, diamondRequestId, null));
                if (createFlag.IsFailed)
                    return Result.Fail(createFlag.Errors);
            }
            else if (diamondId != null && diamondRequest.DiamondId?.Value != diamondId)
            {
                if (!String.IsNullOrEmpty(diamondId))
                {
                    var diamond = await _diamondRepository.GetById(DiamondId.Parse(diamondId));
                    if (diamond == null)
                    {
                        return Result.Fail(DiamondErrors.DiamondNotFoundError);
                    }
                    else if (diamond.Status != ProductStatus.Active || diamond.JewelryId != null)
                    {
                        return Result.Fail(DiamondErrors.DiamondNotAvailable);
                    }
                    else
                    {
                        var matchingFlag = _customizeRequestService.IsAssigningDiamondSpecValid(diamondRequest, diamond);
                        if (matchingFlag.IsFailed)
                        {
                            return Result.Fail(DiamondErrors.DiamondNotValid());
                        }
                        else
                        {
                            var prices = await _diamondServices.GetPrice(diamond.Cut, diamond.DiamondShape, diamond.IsLabDiamond, token);
                            var price = await _diamondServices.GetDiamondPrice(diamond, prices);
                            if (price == null || diamond.IsPriceKnown == false)
                            {
                                return Result.Fail(DiamondErrors.UnknownPrice);
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
                }
                else
                    return Result.Fail(CustomizeRequestErrors.DiamondRequest.DiamondRequestNotFoundError);
            }
            else
            {
                return Result.Fail(CustomizeRequestErrors.DiamondRequest.ConflictedDiamondIdError);
            }
            await _unitOfWork.CommitAsync(token);
            return diamondRequest;
        }
    }
}
