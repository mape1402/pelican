# 🦤 Pelican.Mediator

**Simple. Fast. Reliable message delivery for .NET.**

[![NuGet](https://img.shields.io/nuget/v/Pelican.Mediator.svg)](https://www.nuget.org/packages/Pelican.Mediator/)
[![Build](https://img.shields.io/github/actions/workflow/status/mariodev/pelican/build.yml)]([Workflow runs · mape1402/pelican](https://github.com/mape1402/pelican/actions))
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
    public Task<string> Handle(Ping request, CancellationToken cancellationToken)
        => Task.FromResult($"Pong: {request.Message}");
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
    public async Task<TResponse> Handle(TRequest request, Handler<TResponse> next, CancellationToken ct)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        var response = await next();
        Console.WriteLine($"Handled {typeof(TResponse).Name}");
        return response;
    }
}
```

## 🛠️ Upcoming Features

- **Support for Publisher/Subscriber pattern**
   Enable event-based message distribution with Pub/Sub capabilities.
- **Support for PreProcessors and PostProcessors**
   Add hooks to run logic before and after handling a request, useful for validation, logging, transformations, etc.
