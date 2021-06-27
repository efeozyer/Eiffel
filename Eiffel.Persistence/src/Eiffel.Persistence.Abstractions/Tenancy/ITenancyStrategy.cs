using Microsoft.EntityFrameworkCore;

namespace Eiffel.Persistence.Abstractions
{
    public interface ITenancyStrategy<TContext>
        where TContext : DbContext
    {
        TenancyStrategy Strategy { get; }
        void Execute();
    }
}
