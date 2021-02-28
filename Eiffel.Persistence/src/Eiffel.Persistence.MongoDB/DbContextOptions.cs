using MongoDB.Driver;
using System;

namespace Eiffel.Persistence.MongoDb
{
    public abstract class DbContextOptions
    {
        public abstract string Database { get; set; }
        public abstract Type ContextType { get; }
        public abstract string ConnectionString { get; set; }
        public abstract MongoClientSettings ClientSettings { get; set; }
    }

    public class DbContextOptions<TContext> : DbContextOptions
    {
        public DbContextOptions(MongoClientSettings clientSettings)
        {
            ClientSettings = clientSettings;
        }

        public DbContextOptions(string host, int port)
        {
            ClientSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(host, port)
            };
        }
        public override string Database { get; set; }
        public override string ConnectionString { get; set; }
        public override MongoClientSettings ClientSettings { get; set; }
        public override Type ContextType
        {
            get
            {
                return typeof(TContext);
            }
        }
    }
}
