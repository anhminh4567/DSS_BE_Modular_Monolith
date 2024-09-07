using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Infrastructure.Outbox;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.BackgroundJobs
{
    [DisallowConcurrentExecution]
    internal class ProcessOutboxMessagesWorker : IJob
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<OutboxOptions> _outboxOptions;
        private readonly ILogger<ProcessOutboxMessagesWorker> _logger;

        public ProcessOutboxMessagesWorker(IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork, IOptions<OutboxOptions> outboxOptions, ILogger<ProcessOutboxMessagesWorker> logger)
        {
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
            _outboxOptions = outboxOptions;
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("background job is called");
            return Task.CompletedTask;
        }
    }
}
