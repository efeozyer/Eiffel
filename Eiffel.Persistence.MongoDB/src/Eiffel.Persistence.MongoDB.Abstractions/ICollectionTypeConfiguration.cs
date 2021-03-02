namespace Eiffel.Persistence.MongoDB.Abstractions
{
    public interface ICollectionTypeConfiguration<TEntity>
        where TEntity : class
    {
        void Configure(ICollectionTypeBuilder<TEntity> builder);
    }
}
