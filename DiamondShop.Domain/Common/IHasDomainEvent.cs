using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatvisionRemake.Domain.Common
{
    public interface IHasDomainEvent 
    {
        public List<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
