namespace Eiffel.Persistence.MongoDb.Tests.Mocks
{
    public class MockDbContext : DbContext
    {
        public virtual DbSet<MockUserCollection> Users { get; set; }

        public MockDbContext(DbContextOptions<MockDbContext> options) : base(options)
        {

        }
    }
}
