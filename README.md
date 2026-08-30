[![](https://img.shields.io/nuget/v/soenneker.servicebus.receptors.email.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.servicebus.receptors.email/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.servicebus.receptors.email/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.servicebus.receptors.email/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.servicebus.receptors.email.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.servicebus.receptors.email/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.servicebus.receptors.email/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.servicebus.receptors.email/actions/workflows/codeql.yml)

# Soenneker.ServiceBus.Receptors.Email

Consumes messages from the `email` Azure Service Bus queue and durably hands their body and type to `IEmailSender` through Hangfire.

## Installation

```bash
dotnet add package Soenneker.ServiceBus.Receptors.Email
```

## Prerequisites

Configure the Service Bus connection string at `Azure:ServiceBus:ConnectionString`. The credential needs queue-management and receive permissions because receptor initialization creates the `email` queue when absent and starts a processor.

Hangfire must already have a job storage configured, a server must be running, and `IEmailSender` must be resolvable in the Hangfire job activator's service provider. This package does not configure Hangfire or choose an email sender implementation.

## Register and start

```csharp
using Soenneker.ServiceBus.Receptors.Email.Abstract;
using Soenneker.ServiceBus.Receptors.Email.Registrars;

services.AddEmailsReceptorAsSingleton();
```

Registration alone does not start message processing. Resolve and initialize the receptor during application startup:

```csharp
IEmailsReceptor receptor =
    services.GetRequiredService<IEmailsReceptor>();

await receptor.Init(cancellationToken);
```

Keep the singleton alive for the application lifetime and dispose it during shutdown so its Service Bus processor stops cleanly.

## Message contract

For each message, the receptor passes these values directly to the Hangfire job:

```csharp
IEmailSender.Send(
    messageContent,
    type,
    CancellationToken.None)
```

`messageContent` is the raw Service Bus body converted to a string. `type` is read from `ApplicationProperties["type"]`. The receptor does not deserialize or validate an email DTO; `IEmailSender` owns interpretation of both values.

The job intentionally receives `CancellationToken.None` rather than the short-lived Service Bus delivery token. Hangfire manages the eventual background execution after the broker message has been handled.

## Delivery behavior

The Service Bus message is completed only after Hangfire accepts the job. If enqueueing throws, the exception returns to the processor and the Service Bus message remains unsettled for retry.

Delivery is at least once across the Service Bus-to-Hangfire handoff. If Hangfire stores the job but broker completion subsequently fails, the Service Bus message can be delivered again and enqueue a duplicate job. Email handling should use a stable message identifier or other idempotency mechanism where duplicate sends are unacceptable.
