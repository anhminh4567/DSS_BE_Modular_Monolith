using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.ErrorMessages;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using DiamondShop.Domain.Repositories.TransactionRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Transactions.Commands.UpdatePaymentMethod
{
    public record UpdatePaymentMethodCommand(string methodId, bool changeStatus = false, decimal? setMaxPrice = null) : IRequest<Result<PaymentMethod>>;
    internal class UpdatePaymentMethodCommandHandler : IRequestHandler<UpdatePaymentMethodCommand, Result<PaymentMethod>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public UpdatePaymentMethodCommandHandler(IUnitOfWork unitOfWork, IPaymentMethodRepository paymentMethodRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _unitOfWork = unitOfWork;
            _paymentMethodRepository = paymentMethodRepository;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<PaymentMethod>> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var parsedId = PaymentMethodId.Parse(request.methodId);
            var method = await _paymentMethodRepository.GetById(request.methodId);
            if(method is null)
            {
                return Result.Fail(TransactionErrors.PaymentMethodErrors.NotFoundError);
            }
            if (request.changeStatus)
            {
                method.ChangeStatus();
            }
            if(request.setMaxPrice.HasValue)
            {
                method.ChangeMaxSupportedPrice(request.setMaxPrice.Value);
            }
            throw new NotImplementedException();
        }
    }
}
