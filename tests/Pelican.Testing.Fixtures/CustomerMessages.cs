using Pelican.Mediator;

namespace Pelican.Testing.Fixtures
{
    public sealed record CreateCustomerRequest(string Name) : IRequest<CustomerResponse>;

    public sealed record MissingCustomerRequest : IRequest<CustomerResponse>;

    public sealed record CustomerResponse(string Name);

    public sealed record NoResponseRequest(string Value) : IRequest;
}
