[![](https://img.shields.io/nuget/v/soenneker.servicebus.receptors.email.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.servicebus.receptors.email/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.servicebus.receptors.email/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.servicebus.receptors.email/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.servicebus.receptors.email.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.servicebus.receptors.email/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.servicebus.receptors.email/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.servicebus.receptors.email/actions/workflows/codeql.yml)

# Soenneker.ServiceBus.Receptors.Email

A Hangfire-integrated Service Bus message receptor that deserializes incoming Email messages and enqueues them for webhook processing using a background job.

## Install

```bash
dotnet add package Soenneker.ServiceBus.Receptors.Email
```

## Quick start

```csharp
using Soenneker.ServiceBus.Receptors.Email.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddEmailsReceptorAsSingleton();
```

Adds `IEmailsReceptor` as a singleton service.

## What you get

- `IEmailsReceptor` — A Hangfire-integrated Service Bus message receptor that deserializes incoming Email messages and enqueues them for webhook processing using a background job.
- `EmailsReceptorRegistrar` — A Hangfire-integrated Service Bus message receptor that deserializes incoming Email messages and enqueues them for webhook processing using a background job.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `EmailsReceptorRegistrar.AddEmailsReceptorAsSingleton(services)` | Adds `IEmailsReceptor` as a singleton service. | The same service collection, so additional registrations can be chained. |
