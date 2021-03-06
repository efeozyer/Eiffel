﻿using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public abstract class QueryHandler<TQuery, TReply>
        where TQuery : Query<TReply>
        where TReply : class
    {
        public abstract Task<TReply> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
