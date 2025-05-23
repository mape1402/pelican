namespace Pelican.Mediator
{

    /// <summary>
    /// Delegate that represents an asynchronous operation.
    /// </summary>
    /// <typeparam name="TRequest">Type of request.</typeparam>
    /// <param name="request">Mediator message.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns><see cref="Task"/></returns>
    public delegate Task Next<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;

    /// <summary>
    /// Delegate that represents an asynchronous operation.
    /// </summary>
    /// <typeparam name="TRequest">Type of request.</typeparam>
    /// <typeparam name="TResponse">Type of response.</typeparam>
    /// <param name="request">Mediator message.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns><see cref="Task{TResponse}"/></returns>
    public delegate Task<TResponse> Next<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>;

    /// <summary>
    /// Represents an async continuation for the next task to execute in the pipeline.
    /// </summary>
    /// <typeparam name="TResponse">Type of response.</typeparam>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns><see cref="Task{TResponse}"/></returns>
    public delegate Task<TResponse> Handler<TResponse>(CancellationToken cancellationToken = default);

    /// <summary>
    /// Manages method decorators for Mediator message handler executions.
    /// </summary>
    public interface IPipelineManagement
    {
        /// <summary>
        /// Executes Mediator message handler.
        /// </summary>
        /// <typeparam name="TRequest">Type of request.</typeparam>
        /// <param name="request">Mediator message.</param>
        /// <param name="next">Message handler execution.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns><see cref="Task"/></returns>
        Task Handle<TRequest>(TRequest request, Next<TRequest> next, CancellationToken cancellationToken = default) where TRequest : IRequest;

        /// <summary>
        /// Executes Mediator message handler.
        /// </summary>
        /// <typeparam name="TRequest">Type of request.</typeparam>
        /// <typeparam name="TResponse">Type of response.</typeparam>
        /// <param name="request">Mediator message.</param>
        /// <param name="next">Message handler execution.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns><see cref="Task{TResponse}"/></returns>
        Task<TResponse> Handle<TRequest, TResponse>(TRequest request, Next<TRequest, TResponse> next, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>;
    }
}
