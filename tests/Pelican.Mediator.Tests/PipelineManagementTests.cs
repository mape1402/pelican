namespace Pelican.Mediator.Tests
{
    using NSubstitute;
    using Pelican.Mediator.Internals;

    public class PipelineManagementTests
    {
        [Fact]
        public async Task Handle_Should_Invoke_Pipelines_In_Order_And_Then_Handler()
        {
            // Arrange
            var request = new TestRequest();

            var pipeline1 = Substitute.For<IPipelineBehavior<TestRequest, Void>>();
            var pipeline2 = Substitute.For<IPipelineBehavior<TestRequest, Void>>();

            var executed = new List<string>();

            pipeline1
                .Handle(request, Arg.Any<Handler<Void>>(), Arg.Any<CancellationToken>())
                .Returns(async call =>
                {
                    executed.Add("pipeline1");
                    var next = call.ArgAt<Handler<Void>>(1);
                    return await next.Invoke();
                });

            pipeline2
                .Handle(request, Arg.Any<Handler<Void>>(), Arg.Any<CancellationToken>())
                .Returns(async call =>
                {
                    executed.Add("pipeline2");
                    var next = call.ArgAt<Handler<Void>>(1);
                    return await next.Invoke();
                });

            var serviceProviderMock = Substitute.For<IServiceProvider>();
            serviceProviderMock
                .GetService(typeof(IEnumerable<IPipelineBehavior<TestRequest, Void>>))
                .Returns(new[] { pipeline1, pipeline2 });

            var pipelineManagement = new PipelineManagement(serviceProviderMock);

            // Handler final
            var handlerCalled = false;
            Task<Void> FinalHandler(CancellationToken _)
            {
                handlerCalled = true;
                executed.Add("handler");
                return Task.FromResult(Void.Empty);
            }

            // Act
            await pipelineManagement.Handle(request, (r, t) => FinalHandler(t));

            // Assert
            Assert.True(handlerCalled);
            Assert.Equal(new[] { "pipeline1", "pipeline2", "handler" }, executed);
        }

        [Fact]
        public async Task Handle_WithResponse_Should_Invoke_Pipelines_And_Return_Response()
        {
            // Arrange
            var request = new TestRequestWithResponse();
            var response = "🦤 final result";
            var executionOrder = new List<string>();

            var pipeline1 = Substitute.For<IPipelineBehavior<TestRequestWithResponse, string>>();
            var pipeline2 = Substitute.For<IPipelineBehavior<TestRequestWithResponse, string>>();

            pipeline1
                .Handle(request, Arg.Any<Handler<string>>(), Arg.Any<CancellationToken>())
                .Returns(async call =>
                {
                    executionOrder.Add("pipeline1");
                    var next = call.ArgAt<Handler<string>>(1);
                    return await next.Invoke();
                });

            pipeline2
                .Handle(request, Arg.Any<Handler<string>>(), Arg.Any<CancellationToken>())
                .Returns(async call =>
                {
                    executionOrder.Add("pipeline2");
                    var next = call.ArgAt<Handler<string>>(1);
                    return await next.Invoke();
                });

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider
                .GetService(typeof(IEnumerable<IPipelineBehavior<TestRequestWithResponse, string>>))
                .Returns(new[] { pipeline1, pipeline2 });

            var pipelineManagement = new PipelineManagement(serviceProvider);

            // Simula el handler final
            Task<string> FinalHandler(CancellationToken _)
            {
                executionOrder.Add("handler");
                return Task.FromResult(response);
            }

            // Act
            var result = await pipelineManagement.Handle<TestRequestWithResponse, string>(request, (r, t) => FinalHandler(t));

            // Assert
            Assert.Equal(response, result);
            Assert.Equal(new[] { "pipeline1", "pipeline2", "handler" }, executionOrder);
        }

        public class TestRequest : IRequest { }

        public class TestRequestWithResponse : IRequest<string> { }
    }
}

