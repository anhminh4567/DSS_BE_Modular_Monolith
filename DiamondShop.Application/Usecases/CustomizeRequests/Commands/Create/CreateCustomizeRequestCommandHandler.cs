using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.SendRequest
{
    public record CreateCustomizeRequestCommand(string AccountId, CustomizeModelRequest ModelRequest) : IRequest<Result<CustomizeRequest>>;
    internal class CreateCustomizeRequestCommandHandler : IRequestHandler<CreateCustomizeRequestCommand, Result<CustomizeRequest>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IDiamondRequestRepository _diamondRequestRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondServices _diamondServices;
        private readonly IMainDiamondService _mainDiamondService;
        private readonly IAuthenticationService _authenticationService;

        public CreateCustomizeRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork, IAuthenticationService authenticationService, ISideDiamondRepository sideDiamondRepository, ISizeMetalRepository sizeMetalRepository, IMainDiamondService mainDiamondService, IDiamondRequestRepository diamondRequestRepository, IMainDiamondRepository mainDiamondRepository, IDiamondServices diamondServices)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _sideDiamondRepository = sideDiamondRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _mainDiamondService = mainDiamondService;
            _diamondRequestRepository = diamondRequestRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _diamondServices = diamondServices;
        }

        public async Task<Result<CustomizeRequest>> Handle(CreateCustomizeRequestCommand request, CancellationToken token)
        {
            request.Deconstruct(out string accountId, out CustomizeModelRequest modelRequest);
            modelRequest.Deconstruct(out string jewelryModelId, out string metalId, out string sizeId, out string? sideDiamondOptId, out string? engravedText, out string? engravedFont, out string? note, out List<CustomizeDiamondRequest>? diamondRequests);
            await _unitOfWork.BeginTransactionAsync(token);
            //var account = _authenticationService.
            var modelQuery = _sizeMetalRepository.GetQuery();
            modelQuery = _sizeMetalRepository.QueryInclude(modelQuery, p => p.Model);
            modelQuery = _sizeMetalRepository.QueryFilter(modelQuery, p =>
            p.ModelId == JewelryModelId.Parse(jewelryModelId) &&
            p.SizeId == SizeId.Parse(sizeId) &&
            p.MetalId == MetalId.Parse(metalId));
            var modelOpt = modelQuery.FirstOrDefault();
            if (modelOpt == null)
                return Result.Fail(JewelryModelErrors.SizeMetal.SizeMetalNotFoundError);
            //Check if model allow engraving text
            if (!string.IsNullOrEmpty(engravedText) && !modelOpt.Model.IsEngravable)
                return Result.Fail(JewelryModelErrors.NoEngravingError);
            var sideDiamondOpts = await _sideDiamondRepository.GetByModelId(modelOpt.ModelId);
            SideDiamondOpt? sideDiamondOpt = null;
            CustomizeRequest customizedRequest = null;
            if (sideDiamondOpts != null && sideDiamondOpts.Count > 0)
            {
                var selectedOne = sideDiamondOpts.FirstOrDefault(p => p.Id.Value == sideDiamondOptId);
                if (selectedOne == null)
                    return Result.Fail(JewelryModelErrors.SideDiamond.UnsupportedSideDiamondCaratError);
                sideDiamondOpt = selectedOne;
                await _diamondServices.GetSideDiamondPrice(sideDiamondOpt);
                //Side diamond exist and price is unset
                if (sideDiamondOpt != null && sideDiamondOpt.TotalPrice == 0)
                {
                    customizedRequest = CustomizeRequest.CreatePending(AccountId.Parse(accountId), modelOpt.ModelId, modelOpt.SizeId, modelOpt.MetalId, sideDiamondOpt.Id, engravedText, engravedFont, note);
                }
                //Side diamond exist and price is unset
                else
                    customizedRequest = CustomizeRequest.CreateRequesting(AccountId.Parse(accountId), modelOpt.ModelId, modelOpt.SizeId, modelOpt.MetalId, sideDiamondOpt?.Id, engravedText, engravedFont, note);
            }
            //No Side diamond
            else
                customizedRequest = CustomizeRequest.CreateRequesting(AccountId.Parse(accountId), modelOpt.ModelId, modelOpt.SizeId, modelOpt.MetalId, null, engravedText, engravedFont, note);
            await _customizeRequestRepository.Create(customizedRequest);
            await _unitOfWork.SaveChangesAsync(token);
            if (diamondRequests != null)
            {
                customizedRequest.SetPending();
                await _customizeRequestRepository.Update(customizedRequest);
                var diamonds = diamondRequests.Select(p => DiamondRequest.Create(customizedRequest.Id, DiamondShapeId.Parse(p.DiamondShapeId), p.clarityFrom, p.clarityTo, p.colorFrom, p.colorTo, p.cutFrom, p.cutTo, p.caratFrom, p.caratTo, p.isLabGrown, p.polish, p.symmetry, p.girdle, p.culet)).ToList();
                var flagMainDiamond = await _mainDiamondService.CheckMatchingDiamond(modelOpt.ModelId, diamonds, _mainDiamondRepository);
                if (flagMainDiamond.IsFailed)
                    return Result.Fail(flagMainDiamond.Errors);
                await _diamondRequestRepository.CreateRange(diamonds);
                await _unitOfWork.SaveChangesAsync(token);
                customizedRequest.DiamondRequests = diamonds;
            }
            await _unitOfWork.CommitAsync(token);
            return customizedRequest;
        }
    }
}
