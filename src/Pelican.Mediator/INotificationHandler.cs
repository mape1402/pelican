namespace Pelican.Mediator
{
    /// <summary>
    /// Represents the consumer of the publisher message.
    /// </summary>
    /// <typeparam name="TNotification"></typeparam>
    public interface INotificationHandler<TNotification> where TNotification : INotification
    {
        /// <summary>
        /// Manages the publisher message.
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Handle(TNotification notification, CancellationToken cancellationToken = default);
    }
}
