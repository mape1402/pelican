namespace Pelican.Testing
{
    /// <summary>
    /// Describes a single request dispatch trace event.
    /// </summary>
    public sealed class PelicanTraceEntry
    {
        internal PelicanTraceEntry(
            PelicanTraceEventKind kind,
            Type requestType,
            Type responseType,
            Type eventType,
            int order,
            Exception exception = null)
        {
            Kind = kind;
            RequestType = requestType;
            ResponseType = responseType;
            EventType = eventType;
            Order = order;
            Exception = exception;
        }

        /// <summary>
        /// Gets the event kind.
        /// </summary>
        public PelicanTraceEventKind Kind { get; }

        /// <summary>
        /// Gets the request type associated with this event.
        /// </summary>
        public Type RequestType { get; }

        /// <summary>
        /// Gets the response type associated with this event.
        /// </summary>
        public Type ResponseType { get; }

        /// <summary>
        /// Gets the concrete event type, such as the handler or behavior type.
        /// </summary>
        public Type EventType { get; }

        /// <summary>
        /// Gets the zero-based execution order for this event.
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Gets the exception captured for failed handler resolution events.
        /// </summary>
        public Exception Exception { get; }
    }
}
