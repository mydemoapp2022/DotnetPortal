namespace UI.EmployerPortal.Web.Startup.ResiliencyProtocols;

internal interface IAsyncRetryPolicy
{
    Task ExecuteAsync(Func<Task> func);
    Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> func);
}

internal interface IAsyncRetryPolicy<T> : IAsyncRetryPolicy
{

}
