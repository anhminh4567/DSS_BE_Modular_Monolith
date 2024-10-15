using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.OrderItems.Command.Prepare
{
    public record PrepareOrderItemCommand() : IRequest<Result>;
    internal class PrepareOrderItemCommandHandler : IRequestHandler<PrepareOrderItemCommand, Result>
    {
        public Task<Result> Handle(PrepareOrderItemCommand request, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
