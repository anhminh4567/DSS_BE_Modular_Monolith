using DiamondShop.Application.Dtos.Requests.Deliveries;
using DiamondShop.Domain.Models.DeliveryFees;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Deliveries.Commands.Create
{
    public record CreateDeliveryFeeCommand(DeliveryFeeType type, string name, decimal cost, ToLocationCity? ToLocationCity, ToDistance? ToDistance) : IRequest<Result<List<DeliveryFee>>>;
    public record CreateManyDeliveryFeeCommand( List<CreateDeliveryFeeCommand> fees  ) : IRequest<Result<List<DeliveryFee>>>;
}
