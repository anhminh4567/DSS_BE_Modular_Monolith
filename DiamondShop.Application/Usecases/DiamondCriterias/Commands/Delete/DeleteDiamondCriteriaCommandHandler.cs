using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Commands.Delete
{
    public record DeleteDiamondCriteriaCommand(string criteriaId) : IRequest<Result>;
    internal class DeleteDiamondCriteriaCommandHandler : IRequestHandler<DeleteDiamondCriteriaCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;

        public DeleteDiamondCriteriaCommandHandler(IUnitOfWork unitOfWork, IDiamondCriteriaRepository diamondCriteriaRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondCriteriaRepository = diamondCriteriaRepository;
        }

        public async Task<Result> Handle(DeleteDiamondCriteriaCommand request, CancellationToken cancellationToken)
        {
            var criteriaId = DiamondCriteriaId.Parse(request.criteriaId);
            var getCriteria = await _diamondCriteriaRepository.GetById(criteriaId);
            if (getCriteria == null)
                return Result.Fail(new NotFoundError("not found this criteria"));
            await _diamondCriteriaRepository.Delete(getCriteria);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
