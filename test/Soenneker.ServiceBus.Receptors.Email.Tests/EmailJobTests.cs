using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Soenneker.Email.Senders.Abstract;
using Soenneker.Flywheel.Communication.Dtos;
using Soenneker.Flywheel.Core.Services.Abstract;
using Soenneker.ServiceBus.Receptors.Email.Registrars;

namespace Soenneker.ServiceBus.Receptors.Email.Tests;

public class EmailJobTests
{
    [Test]
    public async Task EmailJobPreservesPayloadAndCancellation(CancellationToken cancellationToken)
    {
        var jobs = Substitute.For<IJobClient>();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Azure:ServiceBus:Enable"] = "true", ["Background:QueueLength"] = "32"
        }).Build();
        var receptor = new EmailsReceptor(null!, null!, NullLogger<EmailsReceptor>.Instance, config, jobs);
        await receptor.OnMessageReceived("body", "type", cancellationToken);
        await jobs.Received(1).Enqueue(Arg.Is<JobDefinition<EmailJobPayload>>(j => j.Name == "soenneker.email.send.v1"),
            new EmailJobPayload("body", "type"), cancellationToken: cancellationToken);
    }

    [Test]
    public async Task EnqueueFailurePropagatesAfterPersistenceCompletes()
    {
        var jobs = Substitute.For<IJobClient>();
        var pending = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        jobs.Enqueue(Arg.Any<JobDefinition<EmailJobPayload>>(), Arg.Any<EmailJobPayload>()).Returns(pending.Task);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Azure:ServiceBus:Enable"] = "true", ["Background:QueueLength"] = "32"
        }).Build();
        var receptor = new EmailsReceptor(null!, null!, NullLogger<EmailsReceptor>.Instance, config, jobs);
        Task handling = receptor.OnMessageReceived("body", "type").AsTask();
        if (handling.IsCompleted) throw new InvalidOperationException("Message completed before persistence.");
        var failure = new InvalidOperationException("Store unavailable");
        pending.SetException(failure);
        try
        {
            await handling;
        }
        catch (InvalidOperationException e) when (ReferenceEquals(e, failure))
        {
            return;
        }
        throw new InvalidOperationException("Enqueue failure was swallowed.");
    }
    [Test]
    public async Task RegisteredJobInvokesSender(CancellationToken cancellationToken)
    {
        var sender = Substitute.For<IEmailSender>();
        var services = new ServiceCollection();
        services.AddSingleton(sender);
        services.AddEmailsReceptorAsSingleton();
        services.AddEmailsReceptorAsSingleton();
        await using ServiceProvider provider = services.BuildServiceProvider();
        IJobInvoker? invoker = null;
        foreach (IJobInvoker registered in provider.GetServices<IJobInvoker>())
        {
            if (invoker != null) throw new InvalidOperationException("Duplicate email job registration.");
            invoker = registered;
        }
        await invoker!.Invoke(provider, "{\"messageContent\":\"body\",\"type\":\"type\"}", cancellationToken);
        await sender.Received(1).Send("body", "type", cancellationToken);
    }
}
