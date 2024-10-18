using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Commands.UpdateInfo
{
    public record UpdateStartEndDate(string startDate, string endDate);
    public record UpdateDiscountInfoCommand(string? discountId, string? name, int? percent, UpdateStartEndDate? UpdateStartEndDate) : IRequest<Result<Discount>>;
    internal class UpdateDiscountInfoCommandHandler : IRequestHandler<UpdateDiscountInfoCommand, Result<Discount>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountRepository _discountRepository;

        public UpdateDiscountInfoCommandHandler(IUnitOfWork unitOfWork, IDiscountRepository discountRepository)
        {
            _unitOfWork = unitOfWork;
            _discountRepository = discountRepository;
        }

        public async Task<Result<Discount>> Handle(UpdateDiscountInfoCommand request, CancellationToken cancellationToken)
        {
            var parsedId = DiscountId.Parse(request.discountId!);
            var getDiscount = await _discountRepository.GetById(parsedId);
            if (getDiscount is null)
                return Result.Fail(new NotFoundError());
            //if(getDiscount.Status == Status)
            if (request.name != null)
                getDiscount.Name = request.name;

            if (request.percent != null)
                getDiscount.SetPercentDiscount(request.percent.Value);

            if (request.UpdateStartEndDate != null)
            {
                DateTime startParsed = DateTime.ParseExact(request.UpdateStartEndDate.startDate, DateTimeFormatingRules.DateTimeFormat, null);
                DateTime endParsed = DateTime.ParseExact(request.UpdateStartEndDate.endDate, DateTimeFormatingRules.DateTimeFormat, null);
                getDiscount.ChangeActiveDate(startParsed, endParsed);
            }
            await _discountRepository.Update(getDiscount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return getDiscount;
        }
    }
}
