using System;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public class User
    {
        public string Name { get; set; }
        public byte Age { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }

        public static User Create(string name, byte age, bool isDeleted = false)
        {
            return new User
            {
                Name = name,
                Age = age,
                IsDeleted = isDeleted,
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}
