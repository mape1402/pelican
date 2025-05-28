# 🦤 Pelican.Mediator

**Simple. Fast. Reliable message delivery for .NET.**

[![Build](https://github.com/mape1402/pelican/actions/workflows/publish.yaml/badge.svg)](https://github.com/mape1402/pelican/actions/workflows/publish.yaml)
[![NuGet](https://img.shields.io/nuget/v/Pelican.Mediator.svg)](https://www.nuget.org/packages/Pelican.Mediator/)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

`Pelican.Mediator` is a lightweight and high-performance [Mediator pattern](https://en.wikipedia.org/wiki/Mediator_pattern) implementation for .NET.  
Designed with pipelines, request handlers, and clean architecture in mind.

---

## ✨ Features

- ⚡ Fast: delegate-based
- 🧩 Supports pipelines (`IPipelineBehavior<TRequest, TResponse>`)
- 🔌 DI-friendly: plug it into any `IServiceProvider`
- 🧼 Zero dependencies (except DI abstractions)
- 🧪 Battle-tested with xUnit & NSubstitute

---

## 📦 Installation

```bash
dotnet add package Pelican.Mediator

```

## 🚀 Quick Start

```c#
public record Ping(string Message) : IRequest<string>;

public class PingHandler : IRequestHandler<Ping, string>
{
    public Task<string> Handle(Ping request, CancellationToken cancellationToken = default)
        => Task.FromResult($"Pong: {request.Message}");
}

```

```c#
public record MyNotification(string Message) : INotification;

public class Subscriber1 : INotificationHandler<MyNotification>
{
    public Task Handle(MyNotification notification, CancellationToken = default)
    {
        Console.WriteLine($"Subscriber1: {notification.Message}");
        return Task.CompletedTask;
    }
}

public class Subscriber2 : INotificationHandler<MyNotification>
{
    public Task Handle(MyNotification notification, CancellationToken = default)
    {
        Console.WriteLine($"Subscriber2: {notification.Message}");
        return Task.CompletedTask;
    }
}
```

```c#
public class Generator
{
    private readonly IMediator _mediator;
    
    public Generator(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task Invoke()
    {
        var ping = new PingRequest("Ping");
        var response = await _mediator.Send(ping);
        Console.WriteLine(response); 
        // Output ->
        // Pong: Ping
        
        var notification = new MyNotification("Hello world!");
        await _mediator.Publish(notification);
        // Output ->
	   // Subscriber1: HelloWorld!
	   // Subscriber2: HelloWorld!
    } 
}
```

Add Pelican to the service container.

```c#
services.AddPelican(Assembly.Load("YourProject"));
```

## ⚙️ Pipelines

Create reusable middleware by implementing:

```c#
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    // Support DependencyInjection
    
    public async Task<TResponse> Handle(TRequest request, Handler<TResponse> next, CancellationToken ct)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        var response = await next();
        Console.WriteLine($"Handled {typeof(TResponse).Name}");
        return response;
    }
}
```

Register your pre-processors and post-processors:

```c#
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

## ⚙️ Pre-Processors and Post-Processors

```c#
public class LoggingPreProcessor<TRequest> : IPreProcessor<TRequest>
{
    // Support DependencyInjection
    
    public Task Handle(TRequest request, CancellationToken ct)
    {
        Console.WriteLine($"This is a pre-processor!");        
        return Task.CompletedTask;
    }
}
```

```c#
public class LoggingPostProcessor<TRequest, TResponse> : IPostProcessor<TRequest, TResponse>
{
    // Support DependencyInjection
    
    public Task Handle(TRequest request, TResponse CancellationToken ct)
    {
        Console.WriteLine($"This is a post-processor!");        
        return Task.CompletedTask;
    }
}
```

Register your pre-processors and post-processors:

```c#
services.AddTransient(typeof(IPreProcessor<>), typeof(LoggingPreProcessor<>));
services.AddTransient(typeof(IPostProcessor<,>), typeof(LoggingPostProcessor<,>));
```

## 🛠️ Upcoming Features

- **Extended Base Handlers for Commands and Queries**
   Add abstract classes with basic CRUD operations with Entity Framework Core.
