using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Commands.Create
{
    public record CreateDiscountCommand(string name, string startDate, string endDate, int percent, string? code) : IRequest<Result<Discount>>;
    internal class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, Result<Discount>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountRepository _discountRepository;
        private readonly IRequirementRepository _requirementRepository;

        public CreateDiscountCommandHandler(IUnitOfWork unitOfWork, IDiscountRepository discountRepository, IRequirementRepository requirementRepository)
        {
            _unitOfWork = unitOfWork;
            _discountRepository = discountRepository;
            _requirementRepository = requirementRepository;
        }

        public async Task<Result<Discount>> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            var startDate = DateTime.ParseExact(request.startDate, DateTimeFormatingRules.DateTimeFormat, null);
            var endDate = DateTime.ParseExact(request.endDate, DateTimeFormatingRules.DateTimeFormat, null);
            var newDiscount = Discount.Create(request.name, startDate, endDate, request.percent, request.code);
            await _discountRepository.Create(newDiscount);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(newDiscount);
        }
    }
}
