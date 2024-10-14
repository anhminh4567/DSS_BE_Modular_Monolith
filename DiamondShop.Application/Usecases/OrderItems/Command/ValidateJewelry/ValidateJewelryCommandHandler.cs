using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.OrderItems.Command.ValidateJewelry
{
    public record ValidateJewelryCommand(JewelryId? JewelryId, JewelryModelId? ModelId, List<DiamondId>? DiamondIds, string? EngravedText, string? EngravedFont) : IRequest<Result>;
    internal class ValidateJewelryCommandHandler : IRequestHandler<ValidateJewelryCommand, Result>
    {
        public Task<Result> Handle(ValidateJewelryCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
