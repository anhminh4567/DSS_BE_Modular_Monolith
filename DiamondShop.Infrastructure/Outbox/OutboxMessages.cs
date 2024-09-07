using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Outbox
{
    internal class OutboxMessages
    {
        public string Id { get; set; }

        public string Type { get; set; } 

        public string Content { get; set; }  
        public int ProcessTime { get; set; } = 0;
        public DateTime CreationTime { get; set; }

        public DateTime? CompleteTime { get; set; }

        public string? Exception { get; set; }
    }
}

