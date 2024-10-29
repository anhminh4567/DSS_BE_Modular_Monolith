using DiamondShop.Application.Dtos.Requests.Diamonds;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateMany;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Repositories;
using FluentResults;
using FluentValidation.Results;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateFromRange
{
    public record CreateCriteriaFromRangeCommand(float caratFrom ,float caratTo, Cut? Cut = Cut.Excelent, bool IsSideDiamond = false) : IRequest<Result<List<DiamondCriteria>>>;
    internal class CreateCriteriaFromRangeCommandHandler : IRequestHandler<CreateCriteriaFromRangeCommand, Result<List<DiamondCriteria>>>
    {
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly ISender _sender;
        private const Cut DEFAULT_CUT = Cut.Excelent;
        public CreateCriteriaFromRangeCommandHandler(IDiamondCriteriaRepository diamondCriteriaRepository, ISender sender)
        {
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _sender = sender;
        }

        public async Task<Result<List<DiamondCriteria>>> Handle(CreateCriteriaFromRangeCommand request, CancellationToken cancellationToken)
        {
            List<(float CaratFrom, float CaratTo)> allAvailableCaratRange = new();
            if(request.IsSideDiamond == false)
                allAvailableCaratRange = await _diamondCriteriaRepository.GroupAllAvailableCaratRange(cancellationToken);
            else
                allAvailableCaratRange = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCaratRange(cancellationToken);
            var orderedRange = allAvailableCaratRange.OrderBy(x => x.CaratFrom).ToList();
            foreach(var range in orderedRange)
            {
                if (request.caratFrom < range.CaratTo && request.caratTo > range.CaratFrom)
                {
                    return Result.Fail("The given range already exists or overlaps with an existing range in the database");
                }
            }
            //when all is valid
            List<DiamondCriteriaRequestDto> requests = new();
            foreach(var color in Enum.GetValues(typeof(Color)))
            {
                foreach(var clarity in Enum.GetValues(typeof(Clarity)))
                {
                    requests.Add(new DiamondCriteriaRequestDto() 
                    {
                        CaratFrom = request.caratFrom,
                        CaratTo = request.caratTo,
                        Clarity = (Clarity)clarity,
                        Color = (Color)color,
                        Cut = DEFAULT_CUT//request.Cut.Value,
                    });
                }
            }
            var command = new CreateManyDiamondCriteriasCommand(requests, request.IsSideDiamond);
            var result = await _sender.Send(command, cancellationToken);
            return result;
        }
    }
}
