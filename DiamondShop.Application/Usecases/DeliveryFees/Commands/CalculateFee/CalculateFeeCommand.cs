using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Models.DeliveryFees;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.CalculateFee
{
    public record CalculateFeeCommand(string Province,string District, string Ward, string Street) : IRequest<Result<CalculateFeeRepsonse>>;
    public class CalculateFeeRepsonse
    {
        public LocationDistantData LocationDistantData { get; set; }
        public DeliveryFee DeliveryFee { get; set; }    
    }
}
