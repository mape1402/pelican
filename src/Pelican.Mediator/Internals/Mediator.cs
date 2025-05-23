namespace Pelican.Mediator.Internals
{
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed class Mediator : IMediator
    {
        private static readonly MethodInfo InternalSendMethod = typeof(Mediator).GetMethod(nameof(InternalSend), BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo InternalSendAndRespondMethod = typeof(Mediator).GetMethod(nameof(InternalSendAndRespond), BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly IHandlerFactory _handlerFactory;
        private readonly IPipelineManagement _pipelineManagement;

        private readonly ConcurrentDictionary<Type, Func<Mediator, object, CancellationToken, Task>> _sendCache = new();
        private readonly ConcurrentDictionary<(Type, Type), Func<Mediator, object, CancellationToken, Task>> _sendWithResponseCache = new();

        public Mediator(IHandlerFactory handlerFactory, IPipelineManagement pipelineManagement)
        {
            _handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
            _pipelineManagement = pipelineManagement ?? throw new ArgumentNullException(nameof(pipelineManagement));
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var invoker = _sendCache.GetOrAdd(typeof(TRequest), static requestType =>
            {
                var method = InternalSendMethod.MakeGenericMethod(requestType);
                var instanceParam = Expression.Parameter(typeof(Mediator), "instance");
                var requestParam = Expression.Parameter(typeof(object), "request");
                var tokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                var call = Expression.Call(
                    instanceParam,
                    method,
                    Expression.Convert(requestParam, requestType),
                    tokenParam
                );

                var lambda = Expression.Lambda<Func<Mediator, object, CancellationToken, Task>>(call, instanceParam, requestParam, tokenParam);
                return lambda.Compile();
            });

            return invoker(this, request!, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var key = (request.GetType(), typeof(TResponse));

            var invoker = _sendWithResponseCache.GetOrAdd(key, static tuple =>
            {
                var (requestType, responseType) = tuple;

                var method = InternalSendAndRespondMethod.MakeGenericMethod(requestType, responseType);
                var instanceParam = Expression.Parameter(typeof(Mediator), "instance");
                var requestParam = Expression.Parameter(typeof(object), "request");
                var tokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                var call = Expression.Call(
                    instanceParam,
                    method,
                    Expression.Convert(requestParam, requestType),
                    tokenParam
                );

                var lambda = Expression.Lambda<Func<Mediator, object, CancellationToken, Task>>(call, instanceParam, requestParam, tokenParam);
                return lambda.Compile();
            });

            return (Task<TResponse>)invoker(this, request!, cancellationToken);
        }

        private Task InternalSend<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            var handler = _handlerFactory.Create<TRequest>();
            return _pipelineManagement.Handle(request, handler.Handle, cancellationToken);
        }

        private Task<TResponse> InternalSendAndRespond<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
        {
            var handler = _handlerFactory.Create<TRequest, TResponse>();
            return _pipelineManagement.Handle(request, handler.Handle, cancellationToken);
        }
    }
}
