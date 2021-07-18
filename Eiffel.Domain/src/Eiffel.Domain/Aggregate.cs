using System;
using System.Collections.Generic;

namespace Eiffel.Domain
{
    public abstract class Aggregate
    {
        public Guid Id { get; set; }
        protected List<DomainEvent> Events { get;  }
    }
}
