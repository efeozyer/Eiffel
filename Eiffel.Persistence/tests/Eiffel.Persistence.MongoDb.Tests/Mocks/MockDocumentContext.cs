namespace Eiffel.Persistence.MongoDb.Tests.Mocks
{
    public class MockDocumentContext : DbContext
    {
        public virtual DbSet<MockUserCollection> Users { get; set; }

        public MockDocumentContext(DocumentContextOptions<MockDocumentContext> options) : base(options)
        {

        }
    }
}
