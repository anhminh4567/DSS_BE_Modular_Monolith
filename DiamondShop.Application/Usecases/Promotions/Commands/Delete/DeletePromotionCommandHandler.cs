using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Commands.Delete
{
    public record DeletePromotionCommand(string promoId) : IRequest<Result<Promotion>>;
    public class DeletePromotionCommandHandler : IRequestHandler<DeletePromotionCommand, Result<Promotion>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPromotionServices _promotionServices;

        public DeletePromotionCommandHandler(IUnitOfWork unitOfWork, IPromotionRepository promotionRepository, IPromotionServices promotionServices)
        {
            _unitOfWork = unitOfWork;
            _promotionRepository = promotionRepository;
            _promotionServices = promotionServices;
        }

        public async Task<Result<Promotion>> Handle(DeletePromotionCommand request, CancellationToken cancellationToken)
        {
            var parsedId = PromotionId.Parse(request.promoId);
            var tryGet = await _promotionRepository.GetById(parsedId);
            if (tryGet == null)
            {
                return Result.Fail(new NotFoundError("not found requirement with id: " + parsedId.Value));
            }
            if(tryGet.CanBePermanentlyDeleted == false)
            {
                return Result.Fail(new ConflictError("can only be deleted if the promotion is of status: CANCELLED or EXPIRED"));
            }
            await _promotionRepository.Delete(tryGet);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(tryGet);
        }

    }
}
