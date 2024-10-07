using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.JewelrySideDiamonds.Create;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Jewelries.Commands
{
    public record CreateJewelryCommand(List<string> attachedDiamondIds, JewelryRequestDto JewelryRequest, List<JewelrySideDiamondRequestDto> SideDiamonds) : IRequest<Result<Jewelry>>;
    internal class CreateJewelryCommandHandler : IRequestHandler<CreateJewelryCommand, Result<Jewelry>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly ISender _sender;
        private readonly IUnitOfWork _unitOfWork;

        public CreateJewelryCommandHandler(IJewelryRepository jewelryRepository, IMainDiamondRepository mainDiamondRepository, ISender sender, IUnitOfWork unitOfWork)
        {
            _jewelryRepository = jewelryRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _sender = sender;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Jewelry>> Handle(CreateJewelryCommand request, CancellationToken token)
        {
            request.Deconstruct(out List<string> attachedDiamondIds, out JewelryRequestDto jewelryRequest, out List<JewelrySideDiamondRequestDto> sideDiamonds);
            var attachedDiamonds = new List<Diamond>();
            await _unitOfWork.BeginTransactionAsync(token);
            var flagDuplicatedSerial = await _jewelryRepository.CheckDuplicatedSerial(jewelryRequest.SerialCode);
            if (flagDuplicatedSerial) return Result.Fail(new ConflictError($"This serial number has already existed ({jewelryRequest.SerialCode})"));

            //TODO: Add diamond shape validation here

            var jewelry = Jewelry.Create
                (
                    JewelryModelId.Parse(jewelryRequest.ModelId),
                    SizeId.Parse(jewelryRequest.SizeId),
                    MetalId.Parse(jewelryRequest.MetalId),
                    jewelryRequest.Weight,
                    jewelryRequest.SerialCode
                );
            await _jewelryRepository.Create(jewelry);

            //TODO: Add diamond update handler call
            if (attachedDiamonds.Count > 0)
            {

            }

            foreach (var sideDiamond in sideDiamonds)
            {
                var flagSideDiamond = await _sender.Send(new CreateJewelrySideDiamondCommand(jewelry.Id, sideDiamond));
                if (flagSideDiamond.IsFailed) return Result.Fail(flagSideDiamond.Errors);
            }

            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return jewelry;
        }
    }
}
