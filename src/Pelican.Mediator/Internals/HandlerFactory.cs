namespace Pelican.Mediator.Internals
{
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class HandlerFactory : IHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public HandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IRequestHandler<TRequest> Create<TRequest>() where TRequest : IRequest
            => _serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();

        public IRequestHandler<TRequest, TResponse> Create<TRequest, TResponse>() where TRequest : IRequest<TResponse>
            => _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
    }
}
