using System;

namespace Eiffel.Persistence.Abstractions
{
    public abstract class Document<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; private set; }
        public string ModifiedBy { get; private set; }
        public bool IsDeleted { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
