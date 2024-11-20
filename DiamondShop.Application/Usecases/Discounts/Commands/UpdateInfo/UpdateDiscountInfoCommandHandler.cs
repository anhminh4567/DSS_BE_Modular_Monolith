using DiamondShop.Application.Dtos.Requests.Promotions;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Promotions.Commands.UpdateInfo;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Entities.ErrorMessages;
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
    public record UpdateDiscountInfoCommand(string? discountId, string? name, int? discountPercent, UpdateStartEndDate? UpdateStartEndDate) : IRequest<Result<Discount>>;
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
                return Result.Fail(DiscountErrors.NotFound);
            //if(getDiscount.Status == Status)
            if (request.name != null)
                getDiscount.Name = request.name;

            if (request.discountPercent != null)
                getDiscount.SetPercentDiscount(request.discountPercent.Value);

            if (request.UpdateStartEndDate != null)
            {
                var parsedStartResults = DateTime.TryParseExact(request.UpdateStartEndDate.startDate, DateTimeFormatingRules.DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime startParsed);
                var parsedEndResults = DateTime.TryParseExact(request.UpdateStartEndDate.endDate, DateTimeFormatingRules.DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime endParsed);
                if (parsedStartResults == true && parsedEndResults == true)
                    getDiscount.ChangeActiveDate(startParsed, endParsed);

                else if (parsedStartResults == true && parsedEndResults == false)
                    getDiscount.ChangeStartDate(startParsed);

                else if (parsedEndResults == true && parsedStartResults == false)
                    getDiscount.ChangeEndDate(endParsed);
            }
                await _discountRepository.Update(getDiscount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return getDiscount;
        }
    }
}
