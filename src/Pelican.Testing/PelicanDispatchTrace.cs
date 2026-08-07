using System.Collections.ObjectModel;

namespace Pelican.Testing
{
    /// <summary>
    /// Captures dispatch, handler, and pipeline execution events for Pelican tests.
    /// </summary>
    public sealed class PelicanDispatchTrace
    {
        private readonly List<PelicanTraceEntry> _entries = new();

        /// <summary>
        /// Gets the recorded trace entries.
        /// </summary>
        public IReadOnlyList<PelicanTraceEntry> Entries => new ReadOnlyCollection<PelicanTraceEntry>(_entries);

        /// <summary>
        /// Clears all recorded trace entries.
        /// </summary>
        public void Clear()
            => _entries.Clear();

        internal void RecordDispatch(Type requestType, Type responseType)
            => Record(PelicanTraceEventKind.Dispatch, requestType, responseType, requestType);

        internal void RecordHandlerSelected(Type requestType, Type responseType, Type handlerType)
            => Record(PelicanTraceEventKind.HandlerSelected, requestType, responseType, handlerType);

        internal void RecordPipelineBehavior(Type requestType, Type responseType, Type behaviorType)
            => Record(PelicanTraceEventKind.PipelineBehavior, requestType, responseType, behaviorType);

        internal void RecordHandlerExecution(Type requestType, Type responseType, Type handlerType)
            => Record(PelicanTraceEventKind.HandlerExecution, requestType, responseType, handlerType);

        internal void RecordHandlerResolutionFailed(Type requestType, Type responseType, Exception exception)
            => Record(PelicanTraceEventKind.HandlerResolutionFailed, requestType, responseType, typeof(FailedHandlerResolution), exception);

        private void Record(PelicanTraceEventKind kind, Type requestType, Type responseType, Type eventType, Exception exception = null)
            => _entries.Add(new PelicanTraceEntry(kind, requestType, responseType, eventType, _entries.Count, exception));
    }
}
