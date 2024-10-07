using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelrySideDiamonds.Create
{
    public record CreateJewelrySideDiamondCommand(JewelryId JewelryId, JewelrySideDiamondRequestDto SideDiamondReq) : IRequest<Result<JewelrySideDiamond>>;
    internal class CreateJewelrySideDiamondCommandHandler : IRequestHandler<CreateJewelrySideDiamondCommand, Result<JewelrySideDiamond>>
    {
        private readonly IJewelrySideDiamondRepository _jewelrySideDiamondRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateJewelrySideDiamondCommandHandler(IJewelrySideDiamondRepository jewelrySideDiamondRepository, IUnitOfWork unitOfWork)
        {
            _jewelrySideDiamondRepository = jewelrySideDiamondRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<JewelrySideDiamond>> Handle(CreateJewelrySideDiamondCommand request, CancellationToken token)
        {
            request.Deconstruct(out JewelryId jewelryId, out JewelrySideDiamondRequestDto sideDiamondReq);
            var sideDiamond = JewelrySideDiamond.Create
                (
                    jewelryId, sideDiamondReq.Carat, sideDiamondReq.Quantity,
                    sideDiamondReq.ColorMin, sideDiamondReq.ColorMax, sideDiamondReq.ClarityMin, sideDiamondReq.ClarityMax, sideDiamondReq.SettingType
                );
            await _unitOfWork.BeginTransactionAsync(token);
            await _jewelrySideDiamondRepository.Create(sideDiamond);
            await _unitOfWork.SaveChangesAsync(token);
            return sideDiamond;
        }
    }
}
