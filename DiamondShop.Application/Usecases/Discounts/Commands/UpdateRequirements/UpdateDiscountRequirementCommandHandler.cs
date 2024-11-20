using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions.Entities.ErrorMessages;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Commands.UpdateRequirements
{
    public record UpdateDiscountRequirementCommand(string? discountId,string[] requirementIDs, bool isRemove = false ) : IRequest<Result>;
    internal class UpdateDiscountRequirementCommandHandler : IRequestHandler<UpdateDiscountRequirementCommand, Result>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IRequirementRepository _requirementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDiscountRequirementCommandHandler(IDiscountRepository discountRepository, IRequirementRepository requirementRepository, IUnitOfWork unitOfWork)
        {
            _discountRepository = discountRepository;
            _requirementRepository = requirementRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateDiscountRequirementCommand request, CancellationToken cancellationToken)
        {
            var discountID = DiscountId.Parse(request.discountId);
            var tryGet = await _discountRepository.GetById(discountID);
            if (tryGet == null)
                return Result.Fail(DiscountErrors.NotFound);
            if(request.requirementIDs.Any() is false ) 
            {
                return Result.Fail("không có requirement nào");
            }
            var requirementIDParsed = request.requirementIDs.Select(x => PromoReqId.Parse(x)).ToList();
            var getRequirements = await _requirementRepository.GetRange(requirementIDParsed,cancellationToken);
            if(getRequirements.Any() is false ) 
            {
                return Result.Fail("không có requirement nào");
            }
            getRequirements.ForEach(req => tryGet.SetRequirement(req,request.isRemove));
            await _discountRepository.Update(tryGet);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
