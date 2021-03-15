﻿using Eiffel.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Eiffel.Persistence.Tenancy
{
    public class TenancyDbContext : DbContext, ITenantDbContext<TenantEntity>
    {
        public virtual DbSet<TenantEntity> Tenants { get; set; }

        public TenancyDbContext(DbContextOptions<TenancyDbContext> options) : base(options)
        {

        }
    }
}
