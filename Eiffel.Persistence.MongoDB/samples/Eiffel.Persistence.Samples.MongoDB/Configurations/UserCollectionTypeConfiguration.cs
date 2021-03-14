using Eiffel.Persistence.MongoDB.Abstractions;
using Eiffel.Persistence.Samples.MongoDB.Collections;
using System.Collections.Generic;

namespace Eiffel.Persistence.Samples.MongoDB.Configurations
{
    public class UserCollectionTypeConfiguration : ICollectionTypeConfiguration<User>
    {
        public void Configure(ICollectionTypeBuilder<User> builder)
        {
            builder.ToCollection("Users");
            builder.HasIndex(x => x.Name);
            builder.HasQueryFilter(x => x.IsDeleted == false);

            var documents = new List<User>() 
            {
                new User
                {
                    Age = 1,
                    Name = "First User",
                    IsDeleted = false
                },
                new User
                {
                    Age = 2,
                    Name = "Second User",
                    IsDeleted = true
                }
            };
            builder.HasDocuments(documents);
        }
    }
}
