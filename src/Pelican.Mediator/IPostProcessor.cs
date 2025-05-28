namespace Pelican.Mediator
{
    /// <summary>
    /// Allows you to extend the functionality of operations after they are executed by the Mediator.
    /// </summary>
    /// <typeparam name="TRequest">Type of request.</typeparam>
    /// <typeparam name="TResponse">Type of response</typeparam>
    public interface IPostProcessor<TRequest, TResponse>
    {
        /// <summary>
        /// Triggers follow-up operations after Mediator execution.
        /// </summary>
        /// <param name="request">Mediator message.</param>
        /// <param name="response">Response.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns><see cref="Task{TResponse}"/></returns>
        Task Handle(TRequest request, TResponse response, CancellationToken cancellationToken = default);
    }
}
