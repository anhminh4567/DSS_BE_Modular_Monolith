using DiamondShop.Application.Services.Models;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Payment.Command
{
    public record CreatePaymentCommand(PaymentLinkRequest PaymentLinkRequest) : IRequest<Result<PaymentLinkResponse>>;
    internal class CreatePaymentCommandHandler() : IRequestHandler<CreatePaymentCommand, Result<PaymentLinkResponse>>
    {
        
        public Task<Result<PaymentLinkResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
