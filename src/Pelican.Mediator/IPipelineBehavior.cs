namespace Pelican.Mediator
{
    /// <summary>
    /// Pipeline behavior to surround the inner handler.
    /// </summary>
    /// <typeparam name="TRequest">Type of request.</typeparam>
    /// <typeparam name="TResponse">Type of response.</typeparam>
    public interface IPipelineBehavior<TRequest, TResponse>
    {
        /// <summary>
        /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary.
        /// </summary>
        /// <param name="request">Mediator message.</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns><see cref="Task{TResponse}"/></returns>
        Task<TResponse> Handle(TRequest request, Handler<TResponse> next, CancellationToken cancellationToken = default);
    }
}
