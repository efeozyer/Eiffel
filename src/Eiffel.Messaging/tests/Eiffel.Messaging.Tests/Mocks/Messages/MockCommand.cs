using Eiffel.Messaging.Abstractions.Command;
using Eiffel.Messaging.Tests.Mocks.Exceptions;

namespace Eiffel.Messaging.Tests.Mocks.Messages
{
    public class MockCommand : ICommand
    {
    }

    public class MockUnknownCommand : ICommand
    {

    }

    public class MockInvalidCommand : ICommand, IValidatable
    {
        public void Validate()
        {
            throw new ValidationException();
        }
    }
}
