using System;

namespace Eiffel.Persistence.Abstractions
{
    public abstract class Entity<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public DateTime CreatedOn { get; private set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
