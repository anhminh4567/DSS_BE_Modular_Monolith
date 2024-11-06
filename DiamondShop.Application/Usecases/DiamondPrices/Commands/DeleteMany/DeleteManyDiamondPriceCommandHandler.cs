﻿using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using MediatR.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.DeleteMany
{

    public record DeleteDiamondPriceParameter(string criteriaId);
    public record DeleteManyDiamondPriceCommand(List<DeleteDiamondPriceParameter> deleteList, bool isFancy, bool isSideDiamond, bool isLab) : IRequest<Result>;
    internal class DeleteManyDiamondPriceCommandHandler : IRequestHandler<DeleteManyDiamondPriceCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public DeleteManyDiamondPriceCommandHandler(IUnitOfWork unitOfWork, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondPriceRepository diamondPriceRepository, IDiamondShapeRepository diamondShapeRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondShapeRepository = diamondShapeRepository;
        }

        public async Task<Result> Handle(DeleteManyDiamondPriceCommand request, CancellationToken cancellationToken)
        {
            var getAllShape = await _diamondShapeRepository.GetAllIncludeSpecialShape();
            DiamondShape selectedShape;
            //if (request.isSideDiamond is false)
            //{
            selectedShape = request.isFancy
            ? getAllShape.FirstOrDefault(x => x.Id == DiamondShape.FANCY_SHAPES.Id)
             : getAllShape.FirstOrDefault(x => x.Id == DiamondShape.ROUND.Id);
            //}
            //else 
            //{
            //  selectedShape = getAllShape.FirstOrDefault(x => x.Id == DiamondShape.ANY_SHAPES.Id);
            //}

            var parsedList = request.deleteList.Select(x => new DeleteManyParameter
            (
                selectedShape.Id,
                DiamondCriteriaId.Parse(x.criteriaId)
            )).ToList();
            var deleteResult = await _diamondPriceRepository.DeleteMany(parsedList, request.isLab, request.isSideDiamond, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            return deleteResult;

        }
    }
}
