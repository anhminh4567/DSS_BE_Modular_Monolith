using DiamondShop.Application.Dtos.Requests.Promotions;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Discounts.Commands.CreateFull;
using DiamondShop.Application.Usecases.Discounts.Commands.UpdateInfo;
using DiamondShop.Application.Usecases.Discounts.Commands.UpdateRequirements;
using DiamondShop.Application.Usecases.PromotionGifts.Commands.CreateMany;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Application.Usecases.Promotions.Commands.UpdateRequirements;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Entities.ErrorMessages;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Commands.Update
{
    public record UpdateDiscountRequestDto( string? name, int? discountPercent, string? startDate, string? endDate, List<DiscountRequirement>? requirements, List<string>? removedRequirements);
    public record UpdateDiscountInfoRequest(string? name, int? discountPercent, string? startDate, string? endDate);
    public record UpdateDiscountCommand(string discountId, UpdateDiscountInfoRequest discountInfo, List<DiscountRequirement>? addedRquirements, List<string>? removedRequirements) : IRequest<Result<Discount>>;
    internal class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, Result<Discount>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountService _discountService;
        private readonly IBlobFileServices _blobFileServices;
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public UpdateDiscountCommandHandler(IUnitOfWork unitOfWork, IDiscountRepository discountRepository, IDiscountService discountService, IBlobFileServices blobFileServices, ISender sender, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _discountRepository = discountRepository;
            _discountService = discountService;
            _blobFileServices = blobFileServices;
            _sender = sender;
            _mapper = mapper;
        }

        public async Task<Result<Discount>> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        { 
            var parsedId = DiscountId.Parse(request.discountId);
            Discount? getDiscount = await _discountRepository.GetById(parsedId);
            if(getDiscount is null)
                return Result.Fail(DiscountErrors.NotFound);
            await _unitOfWork.BeginTransactionAsync();
            if (request.discountInfo != null)
            {
                UpdateStartEndDate updateStartEndDate = null;
                if(request.discountInfo.startDate != null || request.discountInfo.endDate != null)
                    updateStartEndDate = new UpdateStartEndDate(request.discountInfo.startDate, request.discountInfo.endDate);
                var command = new UpdateDiscountInfoCommand(request.discountId,request.discountInfo.name,request.discountInfo.discountPercent, updateStartEndDate);
                var updatePromoInfoResult = await _sender.Send(command, cancellationToken);
                if (updatePromoInfoResult.IsFailed)
                {
                    await _unitOfWork.RollBackAsync();
                    return Result.Fail(updatePromoInfoResult.Errors);
                }
            }
            if (request.removedRequirements != null && request.removedRequirements.Count > 0 )
            {
                var removeRequirementCommands = new UpdateDiscountRequirementCommand(request.discountId, request.removedRequirements.ToArray(), true );
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
                var mappedRequirement = _mapper.Map<List<RequirementSpec>>(request.addedRquirements);
                var createRequirementsResult = await _sender.Send(new CreateRequirementCommand(mappedRequirement), cancellationToken);
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
                return Result.Fail(DiscountErrors.OrderTargetNotAllowed);
            }
            if(getDiscount.DiscountReq.Count <= 0)
            {
                await _unitOfWork.RollBackAsync();
                return Result.Fail(" requirement phải lớn hơn 0");
            }
            await _discountRepository.Update(getDiscount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            var getNewFullDetail = await _discountRepository.GetById(parsedId);
            return getNewFullDetail;
        }
    }
}
