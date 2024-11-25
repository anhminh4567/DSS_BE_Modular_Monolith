using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Entities.ErrorMessages;
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

namespace DiamondShop.Application.Usecases.Discounts.Commands.Pause
{
    public record PauseDiscountCommand(string discountId) : IRequest<Result<Discount>>;
    internal class PauseDiscountCommandHandler : IRequestHandler<PauseDiscountCommand, Result<Discount>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountService _discountServices;
        private readonly IDiscountRepository _discountRepository;

        public PauseDiscountCommandHandler(IUnitOfWork unitOfWork, IDiscountService discountServices, IDiscountRepository discountRepository)
        {
            _unitOfWork = unitOfWork;
            _discountServices = discountServices;
            _discountRepository = discountRepository;
        }

        public async Task<Result<Discount>> Handle(PauseDiscountCommand request, CancellationToken cancellationToken)
        {
            var parsedId = DiscountId.Parse(request.discountId);
            var getDiscount = await _discountRepository.GetById(parsedId);
            if (getDiscount is null)
                return Result.Fail(DiscountErrors.NotFound);
            if (getDiscount.Status == Status.Scheduled)
            {
                var result = getDiscount.SetActive();
                if (result.IsFailed)
                    return Result.Fail(result.Errors);
            }
            else
            {
                var result = getDiscount.Pause();
                if (result.IsFailed)
                    return Result.Fail(result.Errors);
            }
            await _discountRepository.Update(getDiscount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return getDiscount;

        }
    }
}
