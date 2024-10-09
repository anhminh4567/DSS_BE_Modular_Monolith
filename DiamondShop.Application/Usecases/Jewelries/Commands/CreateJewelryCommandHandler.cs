using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.Diamonds.Commands.AttachToJewelry;
using DiamondShop.Application.Usecases.JewelrySideDiamonds.Create;
using DiamondShop.Application.Usecases.MainDiamonds.Commands.CompareDiamondShape;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Jewelries.Commands
{
    public record CreateJewelryCommand(JewelryRequestDto JewelryRequest, List<JewelrySideDiamondRequestDto>? SideDiamonds, List<DiamondId>? attachedDiamondIds) : IRequest<Result<Jewelry>>;
    internal class CreateJewelryCommandHandler : IRequestHandler<CreateJewelryCommand, Result<Jewelry>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly ISender _sender;
        private readonly IUnitOfWork _unitOfWork;

        public CreateJewelryCommandHandler(IJewelryRepository jewelryRepository, IDiamondRepository diamondRepository, IMainDiamondRepository mainDiamondRepository, ISender sender, IUnitOfWork unitOfWork)
        {
            _jewelryRepository = jewelryRepository;
            _diamondRepository = diamondRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _sender = sender;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Jewelry>> Handle(CreateJewelryCommand request, CancellationToken token)
        {
            request.Deconstruct(out JewelryRequestDto jewelryRequest, out List<JewelrySideDiamondRequestDto>? sideDiamondReqs, out List<DiamondId>? attachedDiamondIds);
            var attachedDiamonds = new List<Diamond>();
            await _unitOfWork.BeginTransactionAsync(token);
            var flagDuplicatedSerial = await _jewelryRepository.CheckDuplicatedSerial(jewelryRequest.SerialCode);
            if (flagDuplicatedSerial) return Result.Fail(new ConflictError($"This serial number has already existed ({jewelryRequest.SerialCode})"));

            if (attachedDiamondIds is not null)
            {
                var diamondQuery = _diamondRepository.GetQuery();
                diamondQuery = _diamondRepository.QueryFilter(diamondQuery, p => attachedDiamondIds.Contains(p.Id));
                attachedDiamonds = diamondQuery.ToList();
                var flagUnmatchedDiamonds = await _sender.Send(new CompareDiamondShapeCommand(jewelryRequest.ModelId, attachedDiamonds));
                if (flagUnmatchedDiamonds.IsFailed) return Result.Fail(flagUnmatchedDiamonds.Errors);
            }

            var jewelry = Jewelry.Create
                (
                    jewelryRequest.ModelId,
                    jewelryRequest.SizeId,
                    jewelryRequest.MetalId,
                    jewelryRequest.Weight,
                    jewelryRequest.SerialCode
                );
            await _jewelryRepository.Create(jewelry);


            if (attachedDiamonds.Count > 0)
            {
                var flagAttachDiamond = await _sender.Send(new AttachDiamondCommand(jewelry.Id, attachedDiamonds));
                if (flagAttachDiamond.IsFailed) return Result.Fail(flagAttachDiamond.Errors);
            }
            if (sideDiamondReqs is not null)
            {
                var sideDiamondOptIds = sideDiamondReqs.Select(p => p.SideDiamondOptId).ToList();
                var flagSideDiamond = await _sender.Send(new CreateJewelrySideDiamondCommand(jewelry.Id, sideDiamondOptIds));
                if (flagSideDiamond.IsFailed) return Result.Fail(flagSideDiamond.Errors);
            }

            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return jewelry;
        }

    }
}
