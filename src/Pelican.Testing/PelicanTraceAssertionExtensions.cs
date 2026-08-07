namespace Pelican.Testing
{
    /// <summary>
    /// Provides assertion helpers for Pelican dispatch traces.
    /// </summary>
    public static class PelicanTraceAssertionExtensions
    {
        /// <summary>
        /// Asserts that a request with a response was handled.
        /// </summary>
        public static PelicanDispatchTrace ShouldHandle<TRequest, TResponse>(this PelicanDispatchTrace trace)
            where TRequest : IRequest<TResponse>
        {
            EnsureTrace(trace);
            var requestType = typeof(TRequest);
            var responseType = typeof(TResponse);

            if (!trace.Entries.Any(x =>
                x.Kind == PelicanTraceEventKind.HandlerExecution
                && x.RequestType == requestType
                && x.ResponseType == responseType))
            {
                throw new PelicanTestingAssertionException(
                    $"Expected request '{requestType.FullName}' with response '{responseType.FullName}' to be handled. Trace: {Describe(trace)}");
            }

            return trace;
        }

        /// <summary>
        /// Asserts that a request without a response was handled.
        /// </summary>
        public static PelicanDispatchTrace ShouldHandle<TRequest>(this PelicanDispatchTrace trace)
            where TRequest : IRequest
        {
            EnsureTrace(trace);
            var requestType = typeof(TRequest);

            if (!trace.Entries.Any(x =>
                x.Kind == PelicanTraceEventKind.HandlerExecution
                && x.RequestType == requestType
                && x.ResponseType == typeof(Pelican.Mediator.Void)))
            {
                throw new PelicanTestingAssertionException(
                    $"Expected request '{requestType.FullName}' without a response to be handled. Trace: {Describe(trace)}");
            }

            return trace;
        }

        /// <summary>
        /// Asserts that the selected handler type handled the request.
        /// </summary>
        public static PelicanDispatchTrace ShouldSelectHandler<THandler>(this PelicanDispatchTrace trace)
        {
            EnsureTrace(trace);
            var handlerType = typeof(THandler);

            if (!trace.Entries.Any(x => x.Kind == PelicanTraceEventKind.HandlerSelected && Matches(x.EventType, handlerType)))
            {
                throw new PelicanTestingAssertionException(
                    $"Expected handler '{handlerType.FullName}' to be selected. Trace: {Describe(trace)}");
            }

            return trace;
        }

        /// <summary>
        /// Asserts that the trace contains the specified pipeline behavior.
        /// </summary>
        public static PelicanDispatchTrace ShouldContainBehavior<TBehavior>(this PelicanDispatchTrace trace)
        {
            EnsureTrace(trace);
            var behaviorType = typeof(TBehavior);

            if (!trace.Entries.Any(x => x.Kind == PelicanTraceEventKind.PipelineBehavior && Matches(x.EventType, behaviorType)))
            {
                throw new PelicanTestingAssertionException(
                    $"Expected pipeline behavior '{behaviorType.FullName}' to execute. Trace: {Describe(trace)}");
            }

            return trace;
        }

        /// <summary>
        /// Asserts that the first behavior or marker ran before the second behavior or marker.
        /// </summary>
        public static PelicanDispatchTrace ShouldRunBehaviorBefore<TFirst, TSecond>(this PelicanDispatchTrace trace)
        {
            EnsureTrace(trace);
            var firstType = typeof(TFirst);
            var secondType = typeof(TSecond);
            var first = FindOrderedEntry(trace, firstType);
            var second = FindOrderedEntry(trace, secondType);

            if (first == null)
            {
                throw new PelicanTestingAssertionException($"Expected '{firstType.FullName}' to appear in trace. Trace: {Describe(trace)}");
            }

            if (second == null)
            {
                throw new PelicanTestingAssertionException($"Expected '{secondType.FullName}' to appear in trace. Trace: {Describe(trace)}");
            }

            if (first.Order >= second.Order)
            {
                throw new PelicanTestingAssertionException(
                    $"Expected '{firstType.FullName}' to run before '{secondType.FullName}'. Trace: {Describe(trace)}");
            }

            return trace;
        }

        /// <summary>
        /// Asserts that handler resolution failed for the request and response pair.
        /// </summary>
        public static PelicanDispatchTrace ShouldFailHandlerResolution<TRequest, TResponse>(this PelicanDispatchTrace trace)
        {
            EnsureTrace(trace);
            var requestType = typeof(TRequest);
            var responseType = typeof(TResponse);

            if (!trace.Entries.Any(x =>
                x.Kind == PelicanTraceEventKind.HandlerResolutionFailed
                && x.RequestType == requestType
                && x.ResponseType == responseType))
            {
                throw new PelicanTestingAssertionException(
                    $"Expected handler resolution to fail for '{requestType.FullName}' with response '{responseType.FullName}'. Trace: {Describe(trace)}");
            }

            return trace;
        }

        /// <summary>
        /// Asserts that handler resolution failed for a request without a response.
        /// </summary>
        public static PelicanDispatchTrace ShouldFailHandlerResolution<TRequest>(this PelicanDispatchTrace trace)
            where TRequest : IRequest
            => trace.ShouldFailHandlerResolution<TRequest, Pelican.Mediator.Void>();

        private static void EnsureTrace(PelicanDispatchTrace trace)
        {
            if (trace == null)
            {
                throw new ArgumentNullException(nameof(trace));
            }
        }

        private static PelicanTraceEntry FindOrderedEntry(PelicanDispatchTrace trace, Type expectedType)
        {
            if (expectedType == typeof(HandlerExecution))
            {
                return trace.Entries.FirstOrDefault(x => x.Kind == PelicanTraceEventKind.HandlerExecution);
            }

            if (expectedType == typeof(FailedHandlerResolution))
            {
                return trace.Entries.FirstOrDefault(x => x.Kind == PelicanTraceEventKind.HandlerResolutionFailed);
            }

            return trace.Entries.FirstOrDefault(x =>
                (x.Kind == PelicanTraceEventKind.PipelineBehavior || x.Kind == PelicanTraceEventKind.HandlerSelected)
                && Matches(x.EventType, expectedType));
        }

        private static bool Matches(Type actualType, Type expectedType)
        {
            if (actualType == expectedType)
            {
                return true;
            }

            return expectedType.IsGenericTypeDefinition
                && actualType.IsGenericType
                && actualType.GetGenericTypeDefinition() == expectedType;
        }

        private static string Describe(PelicanDispatchTrace trace)
            => string.Join(" -> ", trace.Entries.Select(x => $"{x.Order}:{x.Kind}:{x.EventType.Name}"));
    }
}
