namespace Eiffel.Messaging.Abstractions
{
    public interface IMiddlewareProvider
    {
        TResult Get<T, TResult>() where T : class;
    }
}
