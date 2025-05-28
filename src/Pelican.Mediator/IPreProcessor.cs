namespace Pelican.Mediator
{
    /// <summary>
    /// Allows you to extend the functionality of operations before they are executed by the Mediator.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public interface IPreProcessor<TRequest>
    {
        /// <summary>
        /// Triggers pre-processing operations before Mediator execution.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Handle(TRequest request, CancellationToken cancellationToken = default);
    }
}
