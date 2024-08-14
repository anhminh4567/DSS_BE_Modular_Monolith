using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.PipelineBehavior
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public LoggingBehavior(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = request.GetType().Name;
            try
            {
                _logger.LogInformation("Executing command {command}", requestName);

                var result = await next();
                if(result is IResultBase resultType)
                {
                    if(resultType.IsSuccess is false)
                    {
                        _logger.LogInformation("Executing command {command} FAIL", requestName);
                    }
                    else
                    {
                        _logger.LogInformation("Executing command {command} SUCCESS", requestName);
                    }
                }
                else
                {
                    _logger.LogInformation("Executing command {command} Finish", requestName);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Executing command {} EXCEPTION HAPPENED with message: {message}!!!", requestName, ex.Message);
                throw;
            }
        }
    }
}
