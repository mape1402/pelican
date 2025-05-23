namespace Pelican.Mediator.Tests
{
    using NSubstitute;
    using Pelican.Mediator.Internals;

    public class MediatorTests
    {
        private readonly IHandlerFactory _handlerFactory = Substitute.For<IHandlerFactory>();
        private readonly IPipelineManagement _pipelineManagement = Substitute.For<IPipelineManagement>();
        private readonly Mediator _mediator;

        public MediatorTests()
        {
            _mediator = new Mediator(_handlerFactory, _pipelineManagement);
        }

        [Fact]
        public async Task Send_Should_InvokeHandler_For_Request()
        {
            // Arrange
            var request = new TestRequest();
            var handler = Substitute.For<IRequestHandler<TestRequest>>();

            _handlerFactory.Create<TestRequest>().Returns(handler);

            // Evita comparación de delegado por referencia:
            _pipelineManagement
                .Handle(request, Arg.Any<Next<TestRequest>>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    var func = callInfo.ArgAt<Next<TestRequest>>(1);
                    return func.Invoke(request, CancellationToken.None);
                });

            // Configura el comportamiento del handler
            handler.Handle(request, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            await _mediator.Send(request);

            // Assert
            await handler.Received(1).Handle(request, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Send_Should_InvokeHandler_For_RequestWithResponse()
        {
            // Arrange
            var request = new TestRequestWithResponse();
            var expected = "pelican 🦤";

            var handler = Substitute.For<IRequestHandler<TestRequestWithResponse, string>>();
            _handlerFactory.Create<TestRequestWithResponse, string>().Returns(handler);
            _pipelineManagement
                .Handle(request, handler.Handle, Arg.Any<CancellationToken>())
                .Returns(expected);

            // Act
            var result = await _mediator.Send<string>(request);

            // Assert
            Assert.Equal(expected, result);
        }

        // Dummy types
        public class TestRequest : IRequest { }
        public class TestRequestWithResponse : IRequest<string> { }
    }
}
