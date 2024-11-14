using DiamondShop.Application.Dtos.Requests.Deliveries;
using DiamondShop.Domain.Common.Addresses;
using DiamondShop.Domain.Models.DeliveryFees;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.CreateMany
{
    public record CreateDeliveryFeeCommand( string name, decimal cost, string provinceId) : IRequest<Result<List<DeliveryFee>>>;
    public record CreateManyDeliveryFeeCommand(List<CreateDeliveryFeeCommand> fees) : IRequest<Result<List<DeliveryFee>>>;
}
