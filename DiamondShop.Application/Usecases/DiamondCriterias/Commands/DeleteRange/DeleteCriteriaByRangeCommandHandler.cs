using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Repositories;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Commands.DeleteRange
{
    public class DeleteCriteriaByRangeCommandValidator : AbstractValidator<DeleteCriteriaByRangeCommand>
    {
        public DeleteCriteriaByRangeCommandValidator()
        {
            RuleFor(x => x.isSideDiamond).NotNull();
            RuleFor(x => x.caratFrom).Cascade(CascadeMode.Stop).NotNull().GreaterThanOrEqualTo(0);
            RuleFor(x => x.caratFrom).Cascade(CascadeMode.Stop).NotNull().GreaterThanOrEqualTo(0);
            When(x => x.caratFrom != null && x.caratTo != null, () =>
            {
                RuleFor(x => x.caratFrom).LessThan(x => x.caratTo);
            });
        }
    }
    public record DeleteCriteriaByRangeCommand(bool isSideDiamond, float caratFrom, float caratTo) : IRequest<Result>;
    internal class DeleteCriteriaByRangeCommandHandler : IRequestHandler<DeleteCriteriaByRangeCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;

        public DeleteCriteriaByRangeCommandHandler(IUnitOfWork unitOfWork, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondPriceRepository diamondPriceRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondPriceRepository = diamondPriceRepository;
        }
        public async Task<Result> Handle(DeleteCriteriaByRangeCommand request, CancellationToken cancellationToken)
        {
            Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>> getGrooupedCriteria = new();
            if (request.isSideDiamond == false)
                getGrooupedCriteria = await _diamondCriteriaRepository.GroupAllAvailableCriteria(cancellationToken);
            else
                getGrooupedCriteria = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCriteria(cancellationToken);
            var anyCriteriaInRange = getGrooupedCriteria.FirstOrDefault(x => x.Key.CaratFrom == request.caratFrom && x.Key.CaratTo == request.caratTo);
            if (anyCriteriaInRange.Value == null || anyCriteriaInRange.Value.Count == 0)
            {
                return Result.Fail("Criteria not found");
            }
            foreach (var criteria in anyCriteriaInRange.Value)
            {
                _diamondCriteriaRepository.Delete(criteria).Wait();
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
