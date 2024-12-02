using DiamondShop.Application.Dtos.Requests.Diamonds;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ErrorMessages;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
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
    public record CreateManyDiamondCriteriasCommand(List<DiamondCriteriaRequestDto> listCriteria,string diamondShapeId, bool IsSideDiamondCriteria = false) : IRequest<Result<List<DiamondCriteria>>>;
    internal class CreateManyDiamondCriteriasCommandhandler : IRequestHandler<CreateManyDiamondCriteriasCommand, Result<List<DiamondCriteria>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public CreateManyDiamondCriteriasCommandhandler(IUnitOfWork unitOfWork, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondShapeRepository diamondShapeRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondShapeRepository = diamondShapeRepository;
        }

        public async Task<Result<List<DiamondCriteria>>> Handle(CreateManyDiamondCriteriasCommand request, CancellationToken cancellationToken)
        {
            var getAllShape = await _diamondShapeRepository.GetAllIncludeSpecialShape();
            DiamondShape correctShape  = getAllShape.FirstOrDefault(x => x.Id == DiamondShapeId.Parse(request.diamondShapeId));
            //if (correctShape is null)
            //    return Result.Fail(new NotFoundError("Shape not found"));
            List<DiamondCriteria> mappedItems = new();
            if (request.IsSideDiamondCriteria)
            {
                correctShape = getAllShape.FirstOrDefault(x => x.Id == DiamondShape.ANY_SHAPES.Id);
                if (correctShape is null)
                    return Result.Fail(DiamondShapeErrors.NotFoundError);
                mappedItems = request.listCriteria.Select(c => DiamondCriteria.CreateSideDiamondCriteria(c.CaratFrom, c.CaratTo, correctShape)).ToList();
            }
            else
            {
                if (correctShape is null)
                    return Result.Fail(DiamondShapeErrors.NotFoundError);
                bool isFancyShape = correctShape.IsFancy();
                if(isFancyShape)
                    mappedItems = request.listCriteria.Select(c => DiamondCriteria.Create( c.CaratFrom, c.CaratTo, correctShape)).ToList();
                else
                    mappedItems = request.listCriteria.Select(c => DiamondCriteria.Create(c.CaratFrom, c.CaratTo, correctShape)).ToList();
            }

            await _unitOfWork.BeginTransactionAsync();
            await _diamondCriteriaRepository.CreateMany(mappedItems);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return mappedItems;
        }
    }

}
