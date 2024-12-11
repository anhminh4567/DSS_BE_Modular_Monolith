using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Entities.ErrorMessages;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Commands.Delete
{
    public record DeleteDiscountCommand(string discountId) : IRequest<Result<Discount>>;
    internal class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, Result<Discount>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountRepository _discountRepository;
        private readonly IRequirementRepository _requirementRepository;

        public DeleteDiscountCommandHandler(IUnitOfWork unitOfWork, IDiscountRepository discountRepository, IRequirementRepository requirementRepository)
        {
            _unitOfWork = unitOfWork;
            _discountRepository = discountRepository;
            _requirementRepository = requirementRepository;
        }

        public async Task<Result<Discount>> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            var discountId = DiscountId.Parse(request.discountId);
            var tryGetDiscount = await _discountRepository.GetById(discountId);
            if (tryGetDiscount == null)
                return Result.Fail(DiscountErrors.NotFound);
            if(tryGetDiscount.CanBePermanentlyDeleted == false)
                return Result.Fail(DiscountErrors.DeleteUnallowed("chỉ có thể xóa nếu expired hoặc cancelled"));
            tryGetDiscount.SetSoftDelete();
            await _discountRepository.Update(tryGetDiscount);
            //await _discountRepository.Delete(tryGetDiscount);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(tryGetDiscount);
        }
    }
}
