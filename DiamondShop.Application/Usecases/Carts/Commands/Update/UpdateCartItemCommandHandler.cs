using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Carts.Commands.Add;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Carts.Commands.Update
{
    public record UpdateCartItemCommand(string accountId, string cartItemIdToRemove, AddToCartCommand AddCommand) : IRequest<Result<List<CartItem>>>;   
    internal class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result<List<CartItem>>>
    {
        private readonly ICartService _cartService;
        private readonly ISender _sender;

        public UpdateCartItemCommandHandler(ICartService cartService, ISender sender)
        {
            _cartService = cartService;
            _sender = sender;
        }

        public async Task<Result<List<CartItem>>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            var accountId = AccountId.Parse(request.accountId);
            var cartItemId = CartItemId.Parse(request.cartItemIdToRemove);
            // remove then add

            await _cartService.RemoveProduct(accountId, cartItemId);
            var addCommand = request.AddCommand with
            {
                userId = request.accountId
            };
            var addResult = await _sender.Send(addCommand);
            if(addResult.IsSuccess  )
            {
                return Result.Ok(addResult.Value);
            }
            return Result.Fail("fail to add but succeed to remove");
        }
    }
}
