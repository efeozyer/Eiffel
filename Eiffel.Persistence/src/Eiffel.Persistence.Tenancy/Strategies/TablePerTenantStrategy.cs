using Eiffel.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;

namespace Eiffel.Persistence.Tenancy
{
    public class TablePerTenantStrategy<TContext> : ITenancyStrategy<TContext>
        where TContext : DbContext
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
