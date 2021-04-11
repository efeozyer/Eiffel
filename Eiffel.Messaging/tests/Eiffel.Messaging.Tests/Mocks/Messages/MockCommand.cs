using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Tests.Mocks.Exceptions;

namespace Eiffel.Messaging.Tests.Mocks.Messages
{
    public class MockCommand : Command
    {
    }

    public class MockUnknownCommand : Command
    {

    }

    public class MockInvalidCommand : Command, IValidatable
    {
        public void Validate()
        {
            throw new ValidationException();
        }
    }
}
