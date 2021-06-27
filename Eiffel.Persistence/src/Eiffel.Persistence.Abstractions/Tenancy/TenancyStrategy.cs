namespace Eiffel.Persistence.Abstractions
{
    public enum TenancyStrategy
    {
        /// <summary>
        /// Database per tenant strategy
        /// </summary>
        Databse,

        /// <summary>
        /// Schema per tenant strategy
        /// </summary>
        Schema,

        /// <summary>
        /// Table per tenant strategy
        /// </summary>
        Table,

        /// <summary>
        /// Identify tenant data with column filtering (WHERE TenantId = ?)
        /// </summary>
        Column
    }
}
