﻿using DiamondShop.Application.Dtos.Requests.Diamonds;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateMany
{
    public record CreateManyDiamondCriteriasCommand(List<DiamondCriteriaRequestDto> listCriteria, bool IsSideDiamondCriteria = false, bool isFancyShapeCriteria = false) : IRequest<Result<List<DiamondCriteria>>>;
    internal class CreateManyDiamondCriteriasCommandhandler : IRequestHandler<CreateManyDiamondCriteriasCommand, Result<List<DiamondCriteria>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;

        public CreateManyDiamondCriteriasCommandhandler(IUnitOfWork unitOfWork, IDiamondCriteriaRepository diamondCriteriaRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondCriteriaRepository = diamondCriteriaRepository;
        }

        public async Task<Result<List<DiamondCriteria>>> Handle(CreateManyDiamondCriteriasCommand request, CancellationToken cancellationToken)
        {
            List<DiamondCriteria> mappedItems = new();
            if (request.IsSideDiamondCriteria)
                mappedItems = request.listCriteria.Select(c => DiamondCriteria.CreateSideDiamondCriteria(c.CaratFrom, c.CaratTo,c.Clarity,c.Color)).ToList();
            else
            {
                if(request.isFancyShapeCriteria)
                    mappedItems = request.listCriteria.Select(c => DiamondCriteria.Create(null, c.Clarity, c.Color, c.CaratFrom, c.CaratTo)).ToList();
                else
                    mappedItems = request.listCriteria.Select(c => DiamondCriteria.Create(c.Cut, c.Clarity, c.Color, c.CaratFrom, c.CaratTo)).ToList();
            }

            await _unitOfWork.BeginTransactionAsync();
            await _diamondCriteriaRepository.CreateMany(mappedItems);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return mappedItems;
        }
    }

}
