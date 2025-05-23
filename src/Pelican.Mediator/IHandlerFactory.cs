namespace Pelican.Mediator
{
    /// <summary>
    /// Manages the handler creations.
    /// </summary>
    public interface IHandlerFactory
    {
        /// <summary>
        /// Create a new scoped <see cref="IRequestHandler{TRequest}"/> instance..
        /// </summary>
        /// <typeparam name="TRequest">Type of Mediator message.</typeparam>
        /// <returns><see cref="IRequestHandler{TRequest}"/></returns>
        IRequestHandler<TRequest> Create<TRequest>() where TRequest : IRequest;

        /// <summary>
        /// Create a new scoped <see cref="IRequestHandler{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <typeparam name="TRequest">Type of Mediator message.</typeparam>
        /// <typeparam name="TResponse">Type of response.</typeparam>
        /// <returns><see cref="IRequestHandler{TRequest, TResponse}"/></returns>
        IRequestHandler<TRequest, TResponse> Create<TRequest, TResponse>() where TRequest : IRequest<TResponse>;
    }
}
