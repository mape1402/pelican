using Microsoft.Extensions.DependencyInjection;
using Pelican.Mediator;
using Pelican.Testing.Fixtures;

namespace Pelican.Testing.Tests
{
    public class PelicanTestingTests
    {
        [Fact]
        public async Task SendAsync_Should_Dispatch_Request_And_Record_Trace()
        {
            var services = new ServiceCollection();
            services.AddPelicanTesting(typeof(CreateCustomerRequestHandler).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));

            using var provider = services.BuildServiceProvider();
            var pelican = provider.GetRequiredService<IPelicanTesting>();

            var response = await pelican.SendAsync(new CreateCustomerRequest("Ada"));

            Assert.Equal("Ada", response.Name);
            pelican.Trace
                .ShouldHandle<CreateCustomerRequest, CustomerResponse>()
                .ShouldSelectHandler<CreateCustomerRequestHandler>()
                .ShouldContainBehavior<ValidationBehavior<CreateCustomerRequest, CustomerResponse>>()
                .ShouldContainBehavior<AuditBehavior<CreateCustomerRequest, CustomerResponse>>()
                .ShouldRunBehaviorBefore<ValidationBehavior<CreateCustomerRequest, CustomerResponse>, AuditBehavior<CreateCustomerRequest, CustomerResponse>>()
                .ShouldRunBehaviorBefore<AuditBehavior<CreateCustomerRequest, CustomerResponse>, HandlerExecution>();
        }

        [Fact]
        public async Task ReplacePelicanHandler_Should_Use_Replacement_Handler()
        {
            var services = new ServiceCollection();
            services.AddPelicanTesting();
            services.AddTransient<IRequestHandler<CreateCustomerRequest, CustomerResponse>, CreateCustomerRequestHandler>();
            services.ReplacePelicanHandler<CreateCustomerRequest, CustomerResponse, FakeCreateCustomerRequestHandler>();

            using var provider = services.BuildServiceProvider();
            var pelican = provider.GetRequiredService<IPelicanTesting>();

            var response = await pelican.SendAsync(new CreateCustomerRequest("Ada"));

            Assert.Equal("fake-Ada", response.Name);
            pelican.Trace
                .ShouldHandle<CreateCustomerRequest, CustomerResponse>()
                .ShouldSelectHandler<FakeCreateCustomerRequestHandler>();
        }

        [Fact]
        public async Task SendAsync_Should_Record_Failed_Handler_Resolution()
        {
            var services = new ServiceCollection();
            services.AddPelicanTesting();

            using var provider = services.BuildServiceProvider();
            var pelican = provider.GetRequiredService<IPelicanTesting>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => pelican.SendAsync(new MissingCustomerRequest()));

            pelican.Trace.ShouldFailHandlerResolution<MissingCustomerRequest, CustomerResponse>();
        }

        [Fact]
        public async Task SendAsync_Should_Support_No_Response_Requests()
        {
            NoResponseRequestHandler.Handled = false;

            var services = new ServiceCollection();
            services.AddPelicanTesting(typeof(NoResponseRequestHandler).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            using var provider = services.BuildServiceProvider();
            var pelican = provider.GetRequiredService<IPelicanTesting>();

            await pelican.SendAsync(new NoResponseRequest("run"));

            Assert.True(NoResponseRequestHandler.Handled);
            pelican.Trace
                .ShouldHandle<NoResponseRequest>()
                .ShouldContainBehavior<ValidationBehavior<NoResponseRequest, Pelican.Mediator.Void>>()
                .ShouldRunBehaviorBefore<ValidationBehavior<NoResponseRequest, Pelican.Mediator.Void>, HandlerExecution>();
        }
    }

    public sealed class FakeCreateCustomerRequestHandler : IRequestHandler<CreateCustomerRequest, CustomerResponse>
    {
        public Task<CustomerResponse> Handle(CreateCustomerRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new CustomerResponse($"fake-{request.Name}"));
    }
}
