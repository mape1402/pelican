namespace Pelican.Mediator
{
    /// <summary>
    /// Represents the interface that implements Mediator Pattern.
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Searchs and executes handler for the request. 
        /// </summary>
        /// <typeparam name="TRequest">Type of request.</typeparam>
        /// <param name="request">Mediator message.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns><see cref="Task"/></returns>
        Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;

        /// <summary>
        /// Searchs and executes handler for the request and returns the response from handler.
        /// </summary>
        /// <typeparam name="TResponse">Type of response.</typeparam>
        /// <param name="request">Mediator message.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns><see cref="Task{TResponse}"/></returns>
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searchs and exeutes handlers for the notification.
        /// </summary>
        /// <typeparam name="TNotification">Type of notification.</typeparam>
        /// <param name="notification">Publisher notification.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns><see cref="Task"/></returns>
        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;
    }
}
