using DiamondShop.Application.Services.Data;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.PromotionGifts.Commands.Delete
{

    public record DeleteGiftCommand(string giftId) : IRequest<Result<Gift>>;
    public class DeleteGiftCommandHandler : IRequestHandler<DeleteGiftCommand, Result<Gift>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGiftRepository _giftRepository;

        public DeleteGiftCommandHandler(IUnitOfWork unitOfWork, IGiftRepository giftRepository)
        {
            _unitOfWork = unitOfWork;
            _giftRepository = giftRepository;
        }

        public async Task<Result<Gift>> Handle(DeleteGiftCommand request, CancellationToken cancellationToken)
        {
            var parsedId = GiftId.Parse(request.giftId);
            var tryGet = await _giftRepository.GetById(parsedId);
            if (tryGet == null)
            {
                return Result.Fail(new NotFoundError("not found requirement with id: " + parsedId.Value));
            }
            await _giftRepository.Delete(tryGet);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(tryGet);
        }
    }
}
