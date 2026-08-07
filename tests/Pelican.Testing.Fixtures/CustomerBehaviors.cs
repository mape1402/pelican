using Pelican.Mediator;

namespace Pelican.Testing.Fixtures
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public Task<TResponse> Handle(TRequest request, Handler<TResponse> next, CancellationToken cancellationToken = default)
            => next(cancellationToken);
    }

    public sealed class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public Task<TResponse> Handle(TRequest request, Handler<TResponse> next, CancellationToken cancellationToken = default)
            => next(cancellationToken);
    }
}
