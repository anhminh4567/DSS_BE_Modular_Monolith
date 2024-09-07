using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Outbox
{
    internal class OutboxOptions
    {
        public const string Section = "OutboxSetting";
        public int IntervalSeconds { get; set; }
        public int BatchSize { get; set; }
    }
}
