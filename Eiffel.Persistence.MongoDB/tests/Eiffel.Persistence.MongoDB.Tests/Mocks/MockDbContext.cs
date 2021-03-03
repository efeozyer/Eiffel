using Eiffel.Persistence.MongoDB.Abstractions;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public class MockDbContext : DbContext
    {
        public virtual Collection<MockUserCollection> Users { get; set; }

        public MockDbContext(DbContextOptions<MockDbContext> options) : base(options)
        {

        }
    }
}
