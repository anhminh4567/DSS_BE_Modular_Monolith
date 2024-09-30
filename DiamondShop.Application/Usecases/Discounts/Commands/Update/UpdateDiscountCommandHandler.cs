using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.Discounts.Commands.Create;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
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

namespace DiamondShop.Application.Usecases.Discounts.Commands.Update
{
    public record UpdateDiscountCommand(string? discountId, string? name, string? startDate, string? endDate, int? percent) : IRequest<Result<Discount>>;

    internal class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, Result<Discount>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountRepository _discountRepository;
        private readonly IRequirementRepository _requirementRepository;

        public UpdateDiscountCommandHandler(IUnitOfWork unitOfWork, IDiscountRepository discountRepository, IRequirementRepository requirementRepository)
        {
            _unitOfWork = unitOfWork;
            _discountRepository = discountRepository;
            _requirementRepository = requirementRepository;
        }

        public async Task<Result<Discount>> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var discountID = DiscountId.Parse(request.discountId);
            var tryGet = await _discountRepository.GetById(discountID);
            if (tryGet == null)
                return Result.Fail(new NotFoundError("not fojnd discont"));
            var startDate = DateTime.ParseExact(request.startDate, DateTimeFormatingRules.DateTimeFormat, null);
            var endDate = DateTime.ParseExact(request.endDate, DateTimeFormatingRules.DateTimeFormat, null);
            tryGet.Update(request.name, startDate, endDate, request.percent);
            await _discountRepository.Update(tryGet);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(tryGet);
        }
    }
}
