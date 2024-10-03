using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Commands.Create
{
    public record CreateDiamondCriteriaCommand(Cut Cut, Color Color , Clarity Clarity, float caratFrom, float caratTo, bool isLabGrown = true) : IRequest<Result<DiamondCriteria>>;
    internal class CreateDiamondCriteriaCommandHandler : IRequestHandler<CreateDiamondCriteriaCommand, Result<DiamondCriteria>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;

        public CreateDiamondCriteriaCommandHandler(IUnitOfWork unitOfWork, IDiamondCriteriaRepository diamondCriteriaRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondCriteriaRepository = diamondCriteriaRepository;
        }

        public async Task<Result<DiamondCriteria>> Handle(CreateDiamondCriteriaCommand request, CancellationToken cancellationToken)
        {
            DiamondCriteria diamondCriteria = DiamondCriteria.Create(request.Cut,request.Clarity,request.Color,request.caratFrom,request.caratTo,request.isLabGrown);
            await _diamondCriteriaRepository.Create(diamondCriteria);
            await _unitOfWork.SaveChangesAsync();
            return diamondCriteria;
        }
    }
}
