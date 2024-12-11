using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace DiamondShop.Application.Usecases.Jewelries.Commands.Create
{
    public record CreateJewelryCommand(JewelryRequestDto JewelryRequest, string? SideDiamondOptId, List<string>? attachedDiamondIds) : IRequest<Result<Jewelry>>;
    internal class CreateJewelryCommandHandler : IRequestHandler<CreateJewelryCommand, Result<Jewelry>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IJewelryService _jewelryService;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IMainDiamondService _mainDiamondService;
        private readonly ISender _sender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public CreateJewelryCommandHandler(IJewelryRepository jewelryRepository, IJewelryModelRepository jewelryModelRepository, ISizeMetalRepository sizeMetalRepository, IJewelryService jewelryService, IDiamondRepository diamondRepository, IMainDiamondService mainDiamondService, ISender sender, IUnitOfWork unitOfWork, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _jewelryRepository = jewelryRepository;
            _jewelryModelRepository = jewelryModelRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _jewelryService = jewelryService;
            _diamondRepository = diamondRepository;
            _mainDiamondService = mainDiamondService;
            _sender = sender;
            _unitOfWork = unitOfWork;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<Jewelry>> Handle(CreateJewelryCommand request, CancellationToken token)
        {
            DiamondRule diamondRule = _optionsMonitor.CurrentValue.DiamondRule;
            request.Deconstruct(out JewelryRequestDto jewelryRequest, out string? sideDiamondOptId, out List<string>? attachedDiamondIds);
            var modelQuery = _jewelryModelRepository.GetQuery();
            modelQuery = _jewelryModelRepository.QueryInclude(modelQuery, p => p.SideDiamonds);
            modelQuery = _jewelryModelRepository.QueryInclude(modelQuery, p => p.MainDiamonds);
            modelQuery = _jewelryModelRepository.QueryInclude(modelQuery, p => p.SizeMetals);
            var model = modelQuery.FirstOrDefault(p => p.Id == JewelryModelId.Parse(jewelryRequest.ModelId));
            if (model is null) return Result.Fail(JewelryModelErrors.JewelryModelNotFoundError);

            var sizeMetalQuery = _sizeMetalRepository.GetQuery();
            sizeMetalQuery = _sizeMetalRepository.QueryInclude(sizeMetalQuery, p => p.Metal);
            sizeMetalQuery = _sizeMetalRepository.QueryInclude(sizeMetalQuery, p => p.Size);
            var sizeMetal = sizeMetalQuery.FirstOrDefault(p => p.ModelId == model.Id && p.SizeId == SizeId.Parse(jewelryRequest.SizeId) && p.MetalId == MetalId.Parse(jewelryRequest.MetalId));
            if (sizeMetal is null) return Result.Fail(JewelryModelErrors.SizeMetal.SizeMetalNotFoundError);

            var attachedDiamonds = new List<Diamond>();
            await _unitOfWork.BeginTransactionAsync(token);
            var mainDiamondRequiredCount = model.MainDiamonds.Sum(p => p.Quantity);
            if (mainDiamondRequiredCount != 0)
            {
                if (mainDiamondRequiredCount != attachedDiamondIds?.Count)
                    return Result.Fail(JewelryModelErrors.MainDiamond.MainDiamondCountError(mainDiamondRequiredCount));
                if (attachedDiamondIds is not null && attachedDiamondIds.Count > 0)
                {
                    var convertedId = attachedDiamondIds.Select(DiamondId.Parse).ToList();
                    var diamondQuery = _diamondRepository.GetQuery();
                    diamondQuery = _diamondRepository.QueryFilter(diamondQuery, p => convertedId.Contains(p.Id));
                    attachedDiamonds = diamondQuery.ToList();
                    var flagUnmatchedDiamonds = await _mainDiamondService.CheckMatchingDiamond(model.Id, attachedDiamonds);
                    if (flagUnmatchedDiamonds.IsFailed)
                        return Result.Fail(flagUnmatchedDiamonds.Errors);
                }
            }
            var serialCode = jewelryRequest.ModelCode;
            if(String.IsNullOrEmpty(serialCode))
                serialCode = await _jewelryService.GetSerialCode(model, sizeMetal.Metal, sizeMetal.Size);
            var jewelry = Jewelry.Create
          (
              model.Id,
              sizeMetal.SizeId,
              sizeMetal.MetalId,
              sizeMetal.Weight,
              serialCode,
              status: jewelryRequest.Status
          );
            if (model.SideDiamonds.Count == 0 && sideDiamondOptId != null)
                return Result.Fail(JewelryModelErrors.SideDiamond.NoSideDiamondSupportError);
            if (model.SideDiamonds.Count != 0)
            {
                if (sideDiamondOptId == null)
                    return Result.Fail(JewelryModelErrors.SideDiamondNeededError);
                else
                {
                    var sideDiamond = model.SideDiamonds.FirstOrDefault(p => p.Id == SideDiamondOptId.Parse(sideDiamondOptId));
                    if (sideDiamond == null)
                        return Result.Fail(JewelryModelErrors.SideDiamond.SideDiamondOptNotFoundError);
                    if (sideDiamond.AverageCarat > diamondRule.BiggestSideDiamondCarat)
                        return Result.Fail(JewelryErrors.SideDiamond.UnsupportedSideDiamondError);
                    var jewelrySideDiamond = JewelrySideDiamond.Create(sideDiamond);
                    jewelry.SideDiamond = jewelrySideDiamond;
                }
            }

            await _jewelryRepository.Create(jewelry, token);
            await _unitOfWork.SaveChangesAsync(token);
            if (attachedDiamonds.Count > 0)
            {
                foreach (var diamond in attachedDiamonds)
                {
                    if (diamond.JewelryId != null)
                        return Result.Fail(DiamondErrors.DiamondAssignedToJewelryAlready(jewelry.SerialCode));
                    diamond.SetForJewelry(jewelry);
                }

                _diamondRepository.UpdateRange(attachedDiamonds);
                await _unitOfWork.SaveChangesAsync(token);
            }
            jewelry = _jewelryService.AddPrice(jewelry, sizeMetal);
            await _unitOfWork.CommitAsync(token);
            jewelry.Model = model;
            return jewelry;
        }

    }
}
