using DiamondShop.Application.Dtos.Requests.Accounts;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Accounts.Commands.Update
{
    public record ChangedAddress(string[]? removedAddressId, Dictionary<string, AddressRequestDto>? updatedAddress, AddressRequestDto[]? addedAddress);
    public record UpdateUserAccountRequest(FullName? ChangedFullName, ChangedAddress? ChangedAddress, string? newDefaultAddressId ) : IRequest<Result<Account>>;
    public record UpdateUserAccountCommand(string userId, FullName? ChangedFullName, ChangedAddress? ChangedAddress, string? newDefaultAddressId): IRequest<Result<Account>>;
}
