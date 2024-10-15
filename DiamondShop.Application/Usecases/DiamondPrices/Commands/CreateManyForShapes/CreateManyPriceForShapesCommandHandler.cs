using DiamondShop.Domain.Models.DiamondPrices;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.CreateManyForShapes
{
    public record CreateManyPriceForShapesCommand() : IRequest<Result<List<DiamondPrice>>>;
    internal class CreateManyPriceForShapesCommandHandler
    {
    }
}
