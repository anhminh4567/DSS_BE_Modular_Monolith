using DiamondShop.Infrastructure.BackgroundJobs;
using DiamondShop.Infrastructure.Outbox;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options.Setups
{
    internal class QuartzOptionSetup : IConfigureNamedOptions<QuartzOptions>
    {
        private readonly IOptions<OutboxOptions> _outboxOption;

        public QuartzOptionSetup(IOptions<OutboxOptions> outboxOption)
        {
            _outboxOption = outboxOption;
        }

        public void Configure(string? name, QuartzOptions options)
        {
            Configure(options);
        }

        public void Configure(QuartzOptions options)
        {
            string outboxJobName = nameof(ProcessOutboxMessagesWorker);
            options.AddJob<ProcessOutboxMessagesWorker>(config => config.WithIdentity(outboxJobName))
                .AddTrigger(config => config.ForJob(outboxJobName)
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInSeconds(_outboxOption.Value.IntervalSeconds)
                        .RepeatForever()));

            string promotionJobName = nameof(PromotionManagerWorker);
            options.AddJob<PromotionManagerWorker>(config => config.WithIdentity(promotionJobName))
                .AddTrigger(config => config.ForJob(promotionJobName)
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInMinutes(10)
                        .RepeatForever()));

            string discountJobName = nameof(DiscountManagerWorker);
            options.AddJob<PromotionManagerWorker>(config => config.WithIdentity(discountJobName))
                .AddTrigger(config => config.ForJob(discountJobName)
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInMinutes(10)
                        .RepeatForever()));

        }
    }
}
