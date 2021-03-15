using Microsoft.EntityFrameworkCore;

namespace Eiffel.Persistence.Abstractions
{
    public interface ITenantDbContext<TEntity>
        where TEntity : class
    {
        DbSet<TEntity> Tenants { get; set; }
    }
}
