using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Diamonds.Commands.AttachToJewelry;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Jewelries.Commands
{
    public record CreateJewelryCommand(JewelryRequestDto JewelryRequest, string? SideDiamondOptId, List<string>? attachedDiamondIds) : IRequest<Result<Jewelry>>;
    internal class CreateJewelryCommandHandler : IRequestHandler<CreateJewelryCommand, Result<Jewelry>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IJewelryService _jewelryService;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IMainDiamondService _mainDiamondService;
        private readonly ISender _sender;
        private readonly IUnitOfWork _unitOfWork;

        public CreateJewelryCommandHandler(IJewelryRepository jewelryRepository, IJewelryModelRepository jewelryModelRepository, ISizeMetalRepository sizeMetalRepository, IDiamondRepository diamondRepository, ISender sender, IUnitOfWork unitOfWork, IJewelryService jewelryService, IMainDiamondService mainDiamondService, IMainDiamondRepository mainDiamondRepository)
        {
            _jewelryRepository = jewelryRepository;
            _jewelryModelRepository = jewelryModelRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _diamondRepository = diamondRepository;
            _sender = sender;
            _unitOfWork = unitOfWork;
            _jewelryService = jewelryService;
            _mainDiamondService = mainDiamondService;
            _mainDiamondRepository = mainDiamondRepository;
        }

        public async Task<Result<Jewelry>> Handle(CreateJewelryCommand request, CancellationToken token)
        {
            request.Deconstruct(out JewelryRequestDto jewelryRequest, out string? sideDiamondOptId, out List<string>? attachedDiamondIds);
            var modelQuery = _jewelryModelRepository.GetQuery();
            modelQuery = _jewelryModelRepository.QueryInclude(modelQuery, p => p.SideDiamonds);
            modelQuery = _jewelryModelRepository.QueryInclude(modelQuery, p => p.SizeMetals);
            var model = modelQuery.FirstOrDefault(p => p.Id == JewelryModelId.Parse(jewelryRequest.ModelId));
            if (model is null) return Result.Fail(new NotFoundError("Can't find jewelry model object."));

            var sizeMetalQuery = _sizeMetalRepository.GetQuery();
            sizeMetalQuery = _sizeMetalRepository.QueryInclude(sizeMetalQuery, p => p.Metal);
            var sizeMetal = sizeMetalQuery.FirstOrDefault(p => p.ModelId == model.Id && p.SizeId == SizeId.Parse(jewelryRequest.SizeId) && p.MetalId == MetalId.Parse(jewelryRequest.MetalId));
            if (sizeMetal is null) return Result.Fail(new NotFoundError("Can't find jewelry size metal object."));

            var attachedDiamonds = new List<Diamond>();
            await _unitOfWork.BeginTransactionAsync(token);
            var flagDuplicatedSerial = await _jewelryRepository.CheckDuplicatedSerial(jewelryRequest.SerialCode);
            if (flagDuplicatedSerial) return Result.Fail(new ConflictError($"This serial number has already existed ({jewelryRequest.SerialCode})"));

            if (attachedDiamondIds is not null)
            {
                var convertedId = attachedDiamondIds.Select(DiamondId.Parse).ToList();
                if (convertedId.Count == 0)
                    return Result.Fail($"Theres no diamond selected.");
                var diamondQuery = _diamondRepository.GetQuery();
                diamondQuery = _diamondRepository.QueryFilter(diamondQuery, p => convertedId.Contains(p.Id));
                attachedDiamonds = diamondQuery.ToList();
                if (attachedDiamondIds.Count == 0)
                    return Result.Fail($"The selected {(convertedId.Count > 1 ? "diamonds dont" : "diamond doesn't")} exist.");
                if (attachedDiamonds.Any(p => p.JewelryId != null))
                    return Result.Fail("Some diamonds have already attached to other jewelries.");
                var flagUnmatchedDiamonds = await _mainDiamondService.CheckMatchingDiamond(model.Id, attachedDiamonds, _mainDiamondRepository);
                if (flagUnmatchedDiamonds.IsFailed)
                    return Result.Fail(flagUnmatchedDiamonds.Errors);
            }
            var jewelry = Jewelry.Create
               (
                   model.Id,
                   sizeMetal.SizeId,
                   sizeMetal.MetalId,
                   sizeMetal.Weight,
                   jewelryRequest.SerialCode,
                   status: jewelryRequest.Status
               );
            if (sideDiamondOptId is not null)
            {
                if (model.SideDiamonds.Count > 0)
                    return Result.Fail(new ConflictError("This model doesn't have side diamond"));
                var sideDiamond = model.SideDiamonds.FirstOrDefault(p => p.Id == SideDiamondOptId.Parse(sideDiamondOptId));
                if (sideDiamond == null)
                    return Result.Fail(new ConflictError("This side diamond option doesn't exist"));
                var jewelrySideDiamond = JewelrySideDiamond.Create(sideDiamond);
                jewelry.SideDiamond = jewelrySideDiamond;
            }
            await _jewelryRepository.Create(jewelry, token);
            await _unitOfWork.SaveChangesAsync(token);
            if (attachedDiamonds.Count > 0)
            {
                var flagAttachDiamond = await _sender.Send(new AttachDiamondCommand(jewelry.Id, attachedDiamonds));
                if (flagAttachDiamond.IsFailed) return Result.Fail(flagAttachDiamond.Errors);
            }

            await _unitOfWork.CommitAsync(token);
            jewelry = _jewelryService.AddPrice(jewelry, sizeMetal);
            return jewelry;
        }

    }
}
