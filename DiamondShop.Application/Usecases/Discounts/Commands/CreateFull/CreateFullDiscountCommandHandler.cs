using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Discounts.Commands.Create;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Entities.ErrorMessages;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using FluentValidation.Results;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Commands.CreateFull
{
    public record DiscountRequirement(string Name, TargetType TargetType, string? JewelryModelId, DiamondRequirementSpec? DiamondRequirementSpec);
    //(string Name, TargetType TargetType, Operator Operator, decimal? MoneyAmount, int? Quantity, string? JewelryModelID, DiamondRequirementSpec? DiamondRequirementSpec, bool isPromotion = true);
    public record CreateFullDiscountCommand(CreateDiscountCommand CreateDiscount, List<DiscountRequirement> Requirements) : IRequest<Result<Discount>>;
    internal class CreateFullDiscountCommandHandler : IRequestHandler<CreateFullDiscountCommand, Result<Discount>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountRepository _discountRepository;
        private readonly IRequirementRepository _requirementRepository;
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public CreateFullDiscountCommandHandler(IUnitOfWork unitOfWork, IDiscountRepository discountRepository, IRequirementRepository requirementRepository, ISender sender, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _discountRepository = discountRepository;
            _requirementRepository = requirementRepository;
            _sender = sender;
            _mapper = mapper;
        }

        public async Task<Result<Discount>> Handle(CreateFullDiscountCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var createDiscountResult = await _sender.Send(request.CreateDiscount, cancellationToken);
            if (createDiscountResult.IsFailed)
            {
                await _unitOfWork.RollBackAsync(cancellationToken);
                return Result.Fail(createDiscountResult.Errors);
            }
            var mappedRequirement = _mapper.Map<List<RequirementSpec>>(request.Requirements);
            var createManyRequirements = await _sender.Send(new CreateRequirementCommand(mappedRequirement), cancellationToken);
            if (createManyRequirements.IsFailed)
            {
                await _unitOfWork.RollBackAsync(cancellationToken);
                return Result.Fail(createManyRequirements.Errors);
            }
            var discount = createDiscountResult.Value;
            var requirements = createManyRequirements.Value;
            if(requirements.Any(x => x.TargetType == TargetType.Order))
            {
                await _unitOfWork.RollBackAsync(cancellationToken);
                return Result.Fail(DiscountErrors.OrderTargetNotAllowed);
            }
            requirements.ForEach(x => discount.SetRequirement(x));
            await _discountRepository.Update(discount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
            return discount;
        }
    }
}
