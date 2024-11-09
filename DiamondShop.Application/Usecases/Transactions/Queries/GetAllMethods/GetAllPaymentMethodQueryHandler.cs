using DiamondShop.Application.Dtos.Responses.Transactions;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Repositories.TransactionRepo;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Transactions.Queries.GetAllMethods
{
    public record GetAllPaymentMethodQuery() : IRequest<List<PaymentMethodDto>>;
    internal class GetAllPaymentMethodQueryHandler : IRequestHandler<GetAllPaymentMethodQuery, List<PaymentMethodDto>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IMapper _mapper;

        public GetAllPaymentMethodQueryHandler(IPaymentMethodRepository paymentMethodRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IMapper mapper)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _optionsMonitor = optionsMonitor;
            _mapper = mapper;
        }

        public async Task<List<PaymentMethodDto>> Handle(GetAllPaymentMethodQuery request, CancellationToken cancellationToken)
        {
            var result = await _paymentMethodRepository.GetAll();
            return _mapper.Map<List<PaymentMethodDto>>(result);
        }
    }
}
