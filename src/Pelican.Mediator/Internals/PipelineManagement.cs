namespace Pelican.Mediator.Internals
{
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class PipelineManagement : IPipelineManagement
    {
        private readonly IServiceProvider _serviceProvider;

        public PipelineManagement(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Task Handle<TRequest>(TRequest request, Next<TRequest> next, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            Handler<Void> handler = async (CancellationToken t = default) =>
            {
                await next(request, t == default ? cancellationToken : t);
                return Void.Empty;
            };

            return _serviceProvider.GetServices<IPipelineBehavior<TRequest, Void>>()
                            .Reverse()
                            .Aggregate(handler, (nxt, pipeline) => t => pipeline.Handle(request, nxt, t))();
        }

        public Task<TResponse> Handle<TRequest, TResponse>(TRequest request, Next<TRequest, TResponse> next, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
        {
            Handler<TResponse> handler = (t) => next(request, t == default ? cancellationToken : t);

            return _serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>()
                            .Reverse()
                            .Aggregate(handler, (nxt, pipeline) => t => pipeline.Handle(request, nxt, t))();
        }
    }
}
