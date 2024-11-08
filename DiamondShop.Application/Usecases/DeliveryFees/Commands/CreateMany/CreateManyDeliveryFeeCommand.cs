using DiamondShop.Application.Dtos.Requests.Deliveries;
using DiamondShop.Domain.Models.DeliveryFees;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.CreateMany
{
    public record CreateDeliveryFeeCommand( string name, decimal cost, ToLocationCity? ToLocationCity) : IRequest<Result<List<DeliveryFee>>>;
    public record CreateManyDeliveryFeeCommand(List<CreateDeliveryFeeCommand> fees) : IRequest<Result<List<DeliveryFee>>>;
}
