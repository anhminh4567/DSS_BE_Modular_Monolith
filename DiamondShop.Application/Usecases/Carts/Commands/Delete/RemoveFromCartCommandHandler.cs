using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Carts.Commands.Delete
{
    public record RemoveFromCartCommand(string userId, string cartItemId) : IRequest<Result<List<CartItem>>>;
    internal class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, Result<List<CartItem>>>
    {
        private readonly ICartService _cartService;

        public RemoveFromCartCommandHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<Result<List<CartItem>>> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
        {
            var accountId = AccountId.Parse(request.userId);    
            var cartItemId = CartItemId.Parse(request.cartItemId);
            var result = await _cartService.RemoveProduct(accountId, cartItemId);
            return Result.Ok(result);
            
        }
    }
}
