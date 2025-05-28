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
            return HandleCore(request, () =>
            {
                Handler<Void> handler = async (CancellationToken t = default) =>
                {
                    await next(request, t == default ? cancellationToken : t);
                    return Void.Empty;
                };

                return _serviceProvider.GetServices<IPipelineBehavior<TRequest, Void>>()
                                        .Reverse()
                                        .Aggregate(handler, (nxt, pipeline) => t => pipeline.Handle(request, nxt, t))();
            }, cancellationToken);
        }

        public Task<TResponse> Handle<TRequest, TResponse>(TRequest request, Next<TRequest, TResponse> next, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
        {
            return HandleCore(request, () =>
            {
                Handler<TResponse> handler = (t) => next(request, t == default ? cancellationToken : t);

                return _serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>()
                                        .Reverse()
                                        .Aggregate(handler, (nxt, pipeline) => t => pipeline.Handle(request, nxt, t))();
            }, cancellationToken);
        }

        private async Task<TResponse> HandleCore<TRequest, TResponse>(TRequest request, Func<Task<TResponse>> func, CancellationToken cancellationToken)
        {
            var preprocessors = _serviceProvider.GetService<IEnumerable<IPreProcessor<TRequest>>>();

            if(preprocessors != null && preprocessors.Any())
            {
                foreach (var preprocessor in preprocessors)
                    await preprocessor.Handle(request, cancellationToken);
            }

            var response = await func();

            var postprocessors = _serviceProvider.GetService<IEnumerable<IPostProcessor<TRequest, TResponse>>>();

            if(postprocessors != null && postprocessors.Any())
            {
                foreach (var postprocessor in postprocessors)
                    await postprocessor.Handle(request, response, cancellationToken);
            }

            return response;
        }
    }
}
