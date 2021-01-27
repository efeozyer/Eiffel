﻿using Microsoft.Extensions.DependencyInjection;
using Eiffel.Messaging.Core;
using Xunit;
using Eiffel.Messaging.Abstractions.Event;
using Eiffel.Messaging.Tests.Mocks.Messages;
using Eiffel.Messaging.Abstractions.Command;
using Eiffel.Messaging.Abstractions.Query;
using FluentAssertions;
using System.Linq;
using Eiffel.Messaging.Abstractions;

namespace Eiffel.Messaging.Tests
{
    public class MessageDispatcher_ServiceCollection_Tests
    {
        private readonly IServiceCollection _services;

        public MessageDispatcher_ServiceCollection_Tests()
        {
            _services = new ServiceCollection();
            _services.AddMessageDispatcher();
            _services.AddMessageHandlers();
        }

        [Fact]
        public void Should_Register_Handlers()
        {
            // Arrange
            var serviceProvider = _services.BuildServiceProvider();

            // Act
            var eventHandlers = serviceProvider.GetServices<IEventHandler<MockEvent>>();
            var commandHandler = serviceProvider.GetRequiredService<ICommandHandler<MockCommand>>();
            var queryHandler = serviceProvider.GetRequiredService<IQueryHandler<MockQuery, MockQueryResult>>();

            // Assert
            eventHandlers.Should().NotBeNullOrEmpty();
            eventHandlers.Count().Should().Be(2);
            commandHandler.Should().NotBeNull();
            queryHandler.Should().NotBeNull();
        }

        [Fact]
        public void Should_Register_Message_Dispatcher()
        {
            // Arrange
            var serviceProvider = _services.BuildServiceProvider();

            // Act
            var service = serviceProvider.GetService<IMessageDispatcher>();

            // Assert
            service.Should().NotBeNull();
        }
    }
}
