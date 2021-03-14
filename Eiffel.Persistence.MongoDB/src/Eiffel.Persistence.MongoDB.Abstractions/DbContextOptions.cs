using MongoDB.Driver;
using System;

namespace Eiffel.Persistence.MongoDB.Abstractions
{
    public abstract class DbContextOptions
    {
        public abstract string Database { get; set; }
        public abstract Type ContextType { get; }
        public abstract MongoClientSettings ClientSettings { get; set; }
    }
}
