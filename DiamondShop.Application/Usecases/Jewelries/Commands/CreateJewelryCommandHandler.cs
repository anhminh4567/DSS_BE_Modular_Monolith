using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.Diamonds.Commands.AttachToJewelry;
using DiamondShop.Application.Usecases.JewelrySideDiamonds.Create;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System.Linq;

namespace DiamondShop.Application.Usecases.Jewelries.Commands
{
    public record CreateJewelryCommand(JewelryRequestDto JewelryRequest, List<string>? SideDiamondOptIds, List<string>? attachedDiamondIds) : IRequest<Result<Jewelry>>;
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
            request.Deconstruct(out JewelryRequestDto jewelryRequest, out List<string>? SideDiamondOptIds, out List<string>? attachedDiamondIds);
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
            if (jewelryRequest.AttachDiamond)
            {
                var flagDuplicatedSerial = await _jewelryRepository.CheckDuplicatedSerial(jewelryRequest.SerialCode);
                if (flagDuplicatedSerial) return Result.Fail(new ConflictError($"This serial number has already existed ({jewelryRequest.SerialCode})"));

                if (attachedDiamondIds is not null)
                {
                    var diamondQuery = _diamondRepository.GetQuery();
                    var convertedId = attachedDiamondIds.Select(DiamondId.Parse).ToList();
                    diamondQuery = _diamondRepository.QueryFilter(diamondQuery, p => convertedId.Contains(p.Id));
                    attachedDiamonds = diamondQuery.ToList();
                    if (attachedDiamonds.Any(p => p.JewelryId != null))
                        return Result.Fail("Some diamonds have already attached to other jewelries");
                    var flagUnmatchedDiamonds = await _mainDiamondService.CheckMatchingDiamond(model.Id, attachedDiamonds, _mainDiamondRepository);
                    if (flagUnmatchedDiamonds.IsFailed)
                        return Result.Fail(flagUnmatchedDiamonds.Errors);
                }
            }

            var jewelry = Jewelry.Create
                (
                    model.Id,
                    sizeMetal.SizeId,
                    sizeMetal.MetalId,
                    sizeMetal.Weight,
                    jewelryRequest.SerialCode,
                    isActive: jewelryRequest.IsActive
                );
            await _jewelryRepository.Create(jewelry, token);
            await _unitOfWork.SaveChangesAsync(token);

            if (attachedDiamonds.Count > 0)
            {
                var flagAttachDiamond = await _sender.Send(new AttachDiamondCommand(jewelry.Id, attachedDiamonds));
                if (flagAttachDiamond.IsFailed) return Result.Fail(flagAttachDiamond.Errors);
            }
            if (SideDiamondOptIds is not null)
            {
                if (SideDiamondOptIds.Count != model.SideDiamonds.Count) return Result.Fail(new ConflictError("Can't find this side diamond option"));

                var sideDiamondOpts = SideDiamondOptIds.Select(p => SideDiamondOptId.Parse(p)).ToList();
                var flagSideDiamond = await _sender.Send(new CreateJewelrySideDiamondCommand(jewelry.Id, model.SideDiamonds, sideDiamondOpts));
                if (flagSideDiamond.IsFailed) return Result.Fail(flagSideDiamond.Errors);
            }

            await _unitOfWork.CommitAsync(token);
            jewelry = _jewelryService.AddPrice(jewelry, sizeMetal);
            return jewelry;
        }

    }
}
