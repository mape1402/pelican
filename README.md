# 🦤 Pelican.Mediator

**Simple. Fast. Reliable message delivery for .NET.**

[![NuGet](https://img.shields.io/nuget/v/Pelican.Mediator.svg)](https://www.nuget.org/packages/Pelican.Mediator/)
[![Build](https://img.shields.io/github/actions/workflow/status/mariodev/pelican/build.yml)](https://github.com/mariodev/pelican/actions)
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
