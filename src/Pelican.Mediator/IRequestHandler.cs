namespace Pelican.Mediator
{
    /// <summary>
    /// Represents the consumer of the Mediator message.
    /// </summary>
    /// <typeparam name="TRequest">Type of Mediator message.</typeparam>
    public interface IRequestHandler<TRequest> where TRequest : IRequest
    {
        /// <summary>
        /// Manages the Mediator message.
        /// </summary>
        /// <param name="request">Mediator message.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns><see cref="Task"/></returns>
        Task Handle(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents the consumer of the Mediator message.
    /// </summary>
    /// <typeparam name="TRequest">Type of Mediator message.</typeparam>
    /// <typeparam name="TResponse">Type of response.</typeparam>
    public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Manages the Mediator message.
        /// </summary>
        /// <param name="request">Mediator message.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns><see cref="Task{TResponse}"/></returns>
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }
}
