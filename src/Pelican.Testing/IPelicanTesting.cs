namespace Pelican.Testing
{
    /// <summary>
    /// Sends Pelican requests through test infrastructure and exposes the dispatch trace.
    /// </summary>
    public interface IPelicanTesting
    {
        /// <summary>
        /// Gets the trace for requests sent by this dispatcher.
        /// </summary>
        PelicanDispatchTrace Trace { get; }

        /// <summary>
        /// Sends a request that does not return a value.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <param name="request">The request instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the request has been handled.</returns>
        Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;

        /// <summary>
        /// Sends a request and returns its response.
        /// </summary>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="request">The request instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The handler response.</returns>
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}
