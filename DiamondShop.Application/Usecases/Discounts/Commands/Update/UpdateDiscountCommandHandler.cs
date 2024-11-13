using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Discounts.Commands.UpdateInfo;
using DiamondShop.Application.Usecases.Discounts.Commands.UpdateRequirements;
using DiamondShop.Application.Usecases.PromotionGifts.Commands.CreateMany;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Application.Usecases.Promotions.Commands.UpdateRequirements;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
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

namespace DiamondShop.Application.Usecases.Discounts.Commands.Update
{
    public record UpdateDiscountCommand(string discountId,UpdateDiscountInfoCommand discountInfo, List<RequirementSpec>? addedRquirements, List<string>? removedRequirement) : IRequest<Result<Discount>>;
    internal class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, Result<Discount>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountService _discountService;
        private readonly IBlobFileServices _blobFileServices;
        private readonly ISender _sender;

        public UpdateDiscountCommandHandler(IUnitOfWork unitOfWork, IDiscountRepository discountRepository, IDiscountService discountService, IBlobFileServices blobFileServices, ISender sender)
        {
            _unitOfWork = unitOfWork;
            _discountRepository = discountRepository;
            _discountService = discountService;
            _blobFileServices = blobFileServices;
            _sender = sender;
        }

        public async Task<Result<Discount>> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var parsedId = DiscountId.Parse(request.discountId);
            Discount? getDiscount = await _discountRepository.GetById(parsedId);
            if(getDiscount is null)
                return Result.Fail(new NotFoundError("no discount found"));
            await _unitOfWork.BeginTransactionAsync();
            if (request.discountInfo != null)
            {
                var updatePromoInfoResult = await _sender.Send(request.discountInfo, cancellationToken);
                if (updatePromoInfoResult.IsFailed)
                {
                    await _unitOfWork.RollBackAsync();
                    return Result.Fail(updatePromoInfoResult.Errors);
                }
            }
            if (request.removedRequirement != null && request.removedRequirement.Count > 0 )
            {
                var removeRequirementCommands = new UpdateDiscountRequirementCommand(request.discountId, request.removedRequirement.ToArray(), true );
                var removeRequirementResult = await _sender.Send(removeRequirementCommands, cancellationToken);
                if (removeRequirementResult.IsFailed)
                {
                    await _unitOfWork.RollBackAsync();
                    return Result.Fail(removeRequirementResult.Errors);
                }
            }
            List<PromoReq> addedReq = new();
            if (request.addedRquirements != null && request.addedRquirements.Count > 0)
            {
                var createRequirementsResult = await _sender.Send(new CreateRequirementCommand(request.addedRquirements), cancellationToken);
                if (createRequirementsResult.IsFailed)
                {
                    await _unitOfWork.RollBackAsync();
                    return Result.Fail(createRequirementsResult.Errors);
                }
                addedReq = createRequirementsResult.Value;
            }
            addedReq.ForEach(x => getDiscount.SetRequirement(x));
            if (getDiscount.DiscountReq.Where(x => x.TargetType == TargetType.Order).Count() > 0)
            {
                await _unitOfWork.RollBackAsync();
                return Result.Fail(new ConflictError("discount cannot have order requirement"));
            }
            await _discountRepository.Update(getDiscount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return getDiscount;
        }
    }
}
