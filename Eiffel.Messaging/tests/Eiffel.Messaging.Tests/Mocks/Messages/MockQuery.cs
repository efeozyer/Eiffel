﻿using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Tests.Mocks.Exceptions;
using System.Collections.Generic;

namespace Eiffel.Messaging.Tests.Mocks.Messages
{
    internal interface IValidatable
    {
        void Validate();
    }

    public class MockQuery : Query<MockQueryResult>, IValidatable
    {
        private readonly int _skip;
        private readonly int _take;
        private readonly int _count;

        public MockQuery()
        {

        }

        public MockQuery(int skip, int take, int count = 10)
        {
            _skip = skip;
            _take = take;
            _count = count;
        }

        public void Validate()
        {
            if (_count < 10)
            {
                throw new System.Exception("Count must be greater than 10");
            }
        }
    }

    public class MockQueryResult
    {
        public List<object> Items { get; set; }

        public MockQueryResult()
        {
            Items = new List<object>();
        }
    }

    public class MockUnknownQuery : Query<MockUnkownReply>
    {

    }

    public class MockUnkownReply
    {

    }

    public class MockInvalidQuery : Query<object>, IValidatable
    {
        public MockInvalidQuery()
        {

        }
        public void Validate()
        {
            throw new ValidationException();
        }
    }
}
