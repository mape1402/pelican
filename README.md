# Pelican.Mediator

Simple, fast, reliable message delivery for .NET.

[![Build](https://github.com/mape1402/pelican/actions/workflows/CI.yml/badge.svg)](https://github.com/mape1402/pelican/actions/workflows/CI.yml)
[![NuGet](https://img.shields.io/nuget/v/Pelican.Mediator.svg)](https://www.nuget.org/packages/Pelican.Mediator/)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

`Pelican.Mediator` is a lightweight Mediator pattern implementation for .NET applications. It supports request handlers, notifications, dependency injection, pipeline behaviors, pre-processors, and post-processors.

## Packages

```bash
dotnet add package Pelican.Mediator
dotnet add package Pelican.Testing
```

Both packages target:

- `net8.0`
- `net9.0`
- `net10.0`

## Quick Start

Create a request and handler:

```csharp
public sealed record Ping(string Message) : IRequest<string>;

public sealed class PingHandler : IRequestHandler<Ping, string>
{
    public Task<string> Handle(Ping request, CancellationToken cancellationToken = default)
        => Task.FromResult($"Pong: {request.Message}");
}
```

Register Pelican:

```csharp
services.AddPelican(typeof(PingHandler).Assembly);
```

Send a request:

```csharp
public sealed class Generator
{
    private readonly IMediator _mediator;

    public Generator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Invoke()
    {
        var response = await _mediator.Send(new Ping("Ping"));
        Console.WriteLine(response);
    }
}
```

## Notifications

```csharp
public sealed record CustomerCreated(string Name) : INotification;

public sealed class WelcomeEmailHandler : INotificationHandler<CustomerCreated>
{
    public Task Handle(CustomerCreated notification, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Welcome {notification.Name}");
        return Task.CompletedTask;
    }
}
```

Publish the notification:

```csharp
await mediator.Publish(new CustomerCreated("Ada"));
```

## Pipelines

Pipeline behaviors wrap the handler execution:

```csharp
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        Handler<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        var response = await next(cancellationToken);
        Console.WriteLine($"Handled {typeof(TRequest).Name}");
        return response;
    }
}
```

Register behaviors with dependency injection:

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

Requests without a response use `Pelican.Mediator.Void` as the pipeline response type:

```csharp
public sealed class AuditCommandBehavior<TRequest> : IPipelineBehavior<TRequest, Pelican.Mediator.Void>
    where TRequest : IRequest
{
    public Task<Pelican.Mediator.Void> Handle(
        TRequest request,
        Handler<Pelican.Mediator.Void> next,
        CancellationToken cancellationToken = default)
        => next(cancellationToken);
}
```

## Pre-Processors and Post-Processors

```csharp
public sealed class LoggingPreProcessor<TRequest> : IPreProcessor<TRequest>
{
    public Task Handle(TRequest request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Before handler");
        return Task.CompletedTask;
    }
}
```

```csharp
public sealed class LoggingPostProcessor<TRequest, TResponse> : IPostProcessor<TRequest, TResponse>
{
    public Task Handle(TRequest request, TResponse response, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("After handler");
        return Task.CompletedTask;
    }
}
```

Register processors:

```csharp
services.AddTransient(typeof(IPreProcessor<>), typeof(LoggingPreProcessor<>));
services.AddTransient(typeof(IPostProcessor<,>), typeof(LoggingPostProcessor<,>));
```

## Pelican.Testing

`Pelican.Testing` provides a test dispatcher and trace assertions for request dispatch, handler resolution, pipeline behavior, replacement handlers, failures, and no-response requests.

Register handlers from one or more assemblies:

```csharp
services.AddPelicanTesting(typeof(CreateCustomerCommandHandler).Assembly);
```

Send a request through the test dispatcher:

```csharp
var provider = services.BuildServiceProvider();
var pelican = provider.GetRequiredService<IPelicanTesting>();

var response = await pelican.SendAsync(command);
```

Replace a handler for tests:

```csharp
services.ReplacePelicanHandler<CreateCustomerRequest, CustomerResponse, FakeHandler>();
```

Inspect and assert the dispatch trace:

```csharp
var trace = pelican.Trace;

trace.ShouldHandle<CreateCustomerRequest, CustomerResponse>();
trace.ShouldContainBehavior<ValidationBehavior<CreateCustomerRequest, CustomerResponse>>();
trace.ShouldRunBehaviorBefore<ValidationBehavior<CreateCustomerRequest, CustomerResponse>, HandlerExecution>();
```

No-response requests are supported:

```csharp
await pelican.SendAsync(new RebuildCustomerIndex());

pelican.Trace.ShouldHandle<RebuildCustomerIndex>();
```

External test hosts can integrate Pelican testing with a small wrapper:

```csharp
public static class TestHostPelicanExtensions
{
    public static IServiceCollection UsePelicanTesting(
        this IServiceCollection services,
        params Assembly[] assemblies)
        => services.AddPelicanTestingAdapter(assemblies);
}
```
