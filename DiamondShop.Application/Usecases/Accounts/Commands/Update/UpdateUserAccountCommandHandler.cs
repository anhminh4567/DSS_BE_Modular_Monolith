using DiamondShop.Application.Dtos.Requests.Accounts;
using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Commands.Update
{
    
    public record ChangedAddress(string[] removedAddressId , Dictionary<string, AddressRequestDto> updatedAddress, AddressDto[] addedAddress );
    public record UpdateUserAccountCommand(string userId, FullName? ChangedFullName): IRequest<Result<Account>>;
    internal class UpdateUserAccountCommandHandler : IRequestHandler<UpdateUserAccountCommand, Result<Account>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;

        public UpdateUserAccountCommandHandler(IUnitOfWork unitOfWork, IAccountRepository accountRepository)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
        }

        public async Task<Result<Account>> Handle(UpdateUserAccountCommand request, CancellationToken cancellationToken)
        {
            //var userId = UserId.Parse(request.userId);
            throw new NotImplementedException();
        }
    }
}
