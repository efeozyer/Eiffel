using Eiffel.Persistence.MongoDB;
using Eiffel.Persistence.MongoDB.Abstractions;
using Eiffel.Persistence.Samples.MongoDB.Collections;

namespace Eiffel.Persistence.Samples.MongoDB.Context
{
    public class SampleDbContext : DbContext
    {
        public virtual Collection<User> Users { get; set; }
        public virtual Collection<Profile> Profiles { get; set; }

        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }
    }
}
