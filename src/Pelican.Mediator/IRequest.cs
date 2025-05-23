namespace Pelican.Mediator
{
    /// <summary>
    /// Represents a Mediator message.
    /// </summary>
    public interface IRequest
    {
    }

    /// <summary>
    /// Represents a Mediator message.
    /// </summary>
    /// <typeparam name="TResponse">Type of response.</typeparam>
    public interface IRequest<TResponse>
    {
    }
}
