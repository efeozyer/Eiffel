using MongoDB.Driver;
using System;

namespace Eiffel.Persistence.MongoDb
{
    public abstract class DbContext
    {
        public virtual IMongoDatabase Database { get; private set; }

        public virtual IMongoClient Client { get; private set; }

        protected DbContext(DbContextOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            Client = new MongoClient(options.ClientSettings);
            Database = Client.GetDatabase(options.Database);
        }
    }
}
