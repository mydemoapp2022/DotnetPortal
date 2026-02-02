using Polly;
using Polly.Retry;

namespace UI.EmployerPortal.Web.Startup.ResiliencyProtocols;

internal sealed class AsyncRetryPolicyWrapper<T> : IAsyncRetryPolicy<T>
{
    private const int RetryLimit = 3;
    private readonly AsyncRetryPolicy _retryPolicy;

    public AsyncRetryPolicyWrapper(ILogger<T> logger)
    {
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
            RetryLimit,
            x =>
            {
                return TimeSpan.FromMilliseconds(100 * x);
            },
            (exception, timespan, retryCount, _) =>
            {
                logger.LogError("Attempt {0}/{1}, {2}ms. {3}: {4}", retryCount, RetryLimit, timespan.TotalMilliseconds, exception.GetType().Name, exception.Message);
            });
    }

    public Task ExecuteAsync(Func<Task> func)
    {
        return _retryPolicy.ExecuteAsync(func);
    }

    public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> func)
    {
        return _retryPolicy.ExecuteAsync(func);
    }
}
