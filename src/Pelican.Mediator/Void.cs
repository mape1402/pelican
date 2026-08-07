namespace Pelican.Mediator
{
    /// <summary>
    /// Represents the response type used by requests that do not return a value.
    /// </summary>
    public readonly struct Void
    {
        /// <summary>
        /// Gets the empty response value.
        /// </summary>
        public static Void Empty => new Void();

        /// <summary>
        /// Gets a completed task with the empty response value.
        /// </summary>
        public static Task<Void> Task => System.Threading.Tasks.Task.FromResult(Empty);
    }
}
