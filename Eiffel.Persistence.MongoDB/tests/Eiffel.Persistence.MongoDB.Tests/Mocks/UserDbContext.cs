using Eiffel.Persistence.MongoDB.Abstractions;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public class UserDbContext : Abstractions.DbContext
    {
        public virtual Collection<User> Users { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }
    }
}
