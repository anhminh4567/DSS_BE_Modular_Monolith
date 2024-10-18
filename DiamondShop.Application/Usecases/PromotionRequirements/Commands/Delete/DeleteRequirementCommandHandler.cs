using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
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

namespace DiamondShop.Application.Usecases.PromotionRequirements.Commands.Delete
{
    public record DeleteRequirementCommand(string requirementId) : IRequest<Result<PromoReq>>;
    public class DeleteRequirementCommandHandler : IRequestHandler<DeleteRequirementCommand, Result<PromoReq>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequirementRepository _requirementRepository;

        public DeleteRequirementCommandHandler(IUnitOfWork unitOfWork, IRequirementRepository requirementRepository)
        {
            _unitOfWork = unitOfWork;
            _requirementRepository = requirementRepository;
        }

        public async Task<Result<PromoReq>> Handle(DeleteRequirementCommand request, CancellationToken cancellationToken)
        {
            var parsedId = PromoReqId.Parse(request.requirementId);
            var tryGet = await _requirementRepository.GetById(parsedId);
            if(tryGet == null) 
            {
                return Result.Fail(new NotFoundError("not found requirement with id: "+parsedId.Value));
            }
            await _requirementRepository.Delete(tryGet);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(tryGet);
        }

    }
}
