using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Pelican.Testing
{
    /// <summary>
    /// Sends Pelican requests through resolved handlers and pipeline behaviors while recording a trace.
    /// </summary>
    public sealed class PelicanTestingDispatcher : IPelicanTesting, IPelicanTestingAdapter
    {
        private static readonly MethodInfo SendNoResponseCoreMethod = typeof(PelicanTestingDispatcher).GetMethod(nameof(SendNoResponseCoreAsync), BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo SendResponseCoreMethod = typeof(PelicanTestingDispatcher).GetMethod(nameof(SendResponseCoreAsync), BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly ConcurrentDictionary<Type, Func<PelicanTestingDispatcher, object, CancellationToken, Task>> _sendCache = new();
        private readonly ConcurrentDictionary<(Type RequestType, Type ResponseType), Func<PelicanTestingDispatcher, object, CancellationToken, Task>> _sendResponseCache = new();
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PelicanTestingDispatcher"/> class.
        /// </summary>
        public PelicanTestingDispatcher(IServiceProvider serviceProvider, PelicanDispatchTrace trace)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            Trace = trace ?? throw new ArgumentNullException(nameof(trace));
        }

        /// <inheritdoc />
        public PelicanDispatchTrace Trace { get; }

        /// <inheritdoc />
        public Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            var invoker = _sendCache.GetOrAdd(requestType, static type =>
            {
                var method = SendNoResponseCoreMethod.MakeGenericMethod(type);
                return CompileInvoker(method, type);
            });

            return invoker(this, request, cancellationToken);
        }

        /// <inheritdoc />
        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var key = (request.GetType(), typeof(TResponse));
            var invoker = _sendResponseCache.GetOrAdd(key, static tuple =>
            {
                var method = SendResponseCoreMethod.MakeGenericMethod(tuple.RequestType, tuple.ResponseType);
                return CompileInvoker(method, tuple.RequestType);
            });

            return (Task<TResponse>)invoker(this, request, cancellationToken);
        }

        private async Task SendNoResponseCoreAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest
        {
            Trace.RecordDispatch(typeof(TRequest), typeof(Pelican.Mediator.Void));

            IRequestHandler<TRequest> handler;
            try
            {
                handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();
            }
            catch (Exception exception)
            {
                Trace.RecordHandlerResolutionFailed(typeof(TRequest), typeof(Pelican.Mediator.Void), exception);
                throw;
            }

            Trace.RecordHandlerSelected(typeof(TRequest), typeof(Pelican.Mediator.Void), handler.GetType());

            Handler<Pelican.Mediator.Void> next = async token =>
            {
                Trace.RecordHandlerExecution(typeof(TRequest), typeof(Pelican.Mediator.Void), handler.GetType());
                await handler.Handle(request, token == default ? cancellationToken : token);
                return Pelican.Mediator.Void.Empty;
            };

            var pipeline = _serviceProvider
                .GetServices<IPipelineBehavior<TRequest, Pelican.Mediator.Void>>()
                .Reverse()
                .Aggregate(next, (current, behavior) => async token =>
                {
                    Trace.RecordPipelineBehavior(typeof(TRequest), typeof(Pelican.Mediator.Void), behavior.GetType());
                    return await behavior.Handle(request, current, token);
                });

            await pipeline(cancellationToken);
        }

        private async Task<TResponse> SendResponseCoreAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>
        {
            Trace.RecordDispatch(typeof(TRequest), typeof(TResponse));

            IRequestHandler<TRequest, TResponse> handler;
            try
            {
                handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            }
            catch (Exception exception)
            {
                Trace.RecordHandlerResolutionFailed(typeof(TRequest), typeof(TResponse), exception);
                throw;
            }

            Trace.RecordHandlerSelected(typeof(TRequest), typeof(TResponse), handler.GetType());

            Handler<TResponse> next = token =>
            {
                Trace.RecordHandlerExecution(typeof(TRequest), typeof(TResponse), handler.GetType());
                return handler.Handle(request, token == default ? cancellationToken : token);
            };

            var pipeline = _serviceProvider
                .GetServices<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate(next, (current, behavior) => async token =>
                {
                    Trace.RecordPipelineBehavior(typeof(TRequest), typeof(TResponse), behavior.GetType());
                    return await behavior.Handle(request, current, token);
                });

            return await pipeline(cancellationToken);
        }

        private static Func<PelicanTestingDispatcher, object, CancellationToken, Task> CompileInvoker(MethodInfo method, Type requestType)
        {
            var instanceParam = Expression.Parameter(typeof(PelicanTestingDispatcher), "instance");
            var requestParam = Expression.Parameter(typeof(object), "request");
            var tokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

            var call = Expression.Call(
                instanceParam,
                method,
                Expression.Convert(requestParam, requestType),
                tokenParam);

            return Expression
                .Lambda<Func<PelicanTestingDispatcher, object, CancellationToken, Task>>(call, instanceParam, requestParam, tokenParam)
                .Compile();
        }
    }
}
