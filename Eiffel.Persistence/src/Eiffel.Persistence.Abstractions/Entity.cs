using System;

namespace Eiffel.Persistence.Abstractions.Entity
{
    public abstract class Entity<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public DateTime CreatedOn { get; private set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
