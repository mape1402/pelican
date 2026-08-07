using Pelican.Mediator;

namespace Pelican.Testing.Fixtures
{
    public sealed class CreateCustomerRequestHandler : IRequestHandler<CreateCustomerRequest, CustomerResponse>
    {
        public Task<CustomerResponse> Handle(CreateCustomerRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new CustomerResponse(request.Name));
    }

    public sealed class NoResponseRequestHandler : IRequestHandler<NoResponseRequest>
    {
        public static bool Handled { get; set; }

        public Task Handle(NoResponseRequest request, CancellationToken cancellationToken = default)
        {
            Handled = true;
            return Task.CompletedTask;
        }
    }
}
