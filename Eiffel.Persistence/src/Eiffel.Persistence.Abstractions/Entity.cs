using System;

namespace Eiffel.Persistence.Abstractions.Entity
{
    public abstract class Entity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected TKey Id { get; private set; }
        protected DateTime CreatedOn { get; private set; }
        protected string CreatedBy { get; set; }
        protected DateTime? ModifiedOn { get; set; }
        protected string ModifiedBy { get; set; }
        protected bool IsDeleted { get; set; }
    }
}
