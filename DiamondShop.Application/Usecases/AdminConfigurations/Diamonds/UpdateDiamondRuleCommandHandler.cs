using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig.Models;
using DiamondShop.Domain.BusinessRules;
using FluentResults;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Diamonds
{
    public class UpdateDiamondRuleCommand(DiamondRuleRequestDto diamondRuleRequestDto) : IRequest<Result<DiamondRule>>;
    internal class UpdateDiamondRuleCommandHandler : IRequestHandler<UpdateDiamondRuleCommand, Result<DiamondRule>>
    {
        private readonly IMapper _mapper;
        private readonly IDiamondRuleConfigurationService _service;
        private readonly IOptionsMonitor<DiamondRule> _optionsMonitor;

        public UpdateDiamondRuleCommandHandler(IMapper mapper, IDiamondRuleConfigurationService service, IOptionsMonitor<DiamondRule> optionsMonitor)
        {
            _mapper = mapper;
            _service = service;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<DiamondRule>> Handle(UpdateDiamondRuleCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
