namespace Pelican.Testing
{
    /// <summary>
    /// Defines the kinds of entries recorded in a Pelican dispatch trace.
    /// </summary>
    public enum PelicanTraceEventKind
    {
        /// <summary>
        /// A request dispatch started.
        /// </summary>
        Dispatch,

        /// <summary>
        /// A request handler was selected.
        /// </summary>
        HandlerSelected,

        /// <summary>
        /// A pipeline behavior was executed.
        /// </summary>
        PipelineBehavior,

        /// <summary>
        /// The selected handler was executed.
        /// </summary>
        HandlerExecution,

        /// <summary>
        /// Handler resolution failed.
        /// </summary>
        HandlerResolutionFailed
    }
}
