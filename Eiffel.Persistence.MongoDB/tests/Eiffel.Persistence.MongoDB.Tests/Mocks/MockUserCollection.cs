using System;

namespace Eiffel.Persistence.MongoDB.Tests.Mocks
{
    public class MockUserCollection
    {
        public string Name { get; set; }
        public byte Age { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
