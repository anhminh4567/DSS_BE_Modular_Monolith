using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Carts.Queries.GetUserCart
{
    public record class GetUserCartQuery(string userId) : IRequest<List<CartItem>>;
    internal class GetUserCartQueryHandler : IRequestHandler<GetUserCartQuery, List<CartItem>>
    {
        private readonly ICartService _cartService;
        private readonly IAccountRepository _accountRepository;

        public GetUserCartQueryHandler(ICartService cartService, IAccountRepository accountRepository)
        {
            _cartService = cartService;
            _accountRepository = accountRepository;
        }

        public async Task<List<CartItem>> Handle(GetUserCartQuery request, CancellationToken cancellationToken)
        {
            var accountId = AccountId.Parse(request.userId);
            var getCart = await _cartService.GetCartModel(accountId);
            return getCart;
        }
    }
}
