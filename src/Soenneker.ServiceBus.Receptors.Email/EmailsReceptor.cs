using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Email.Senders.Abstract;
using Soenneker.ServiceBus.Client.Abstract;
using Soenneker.ServiceBus.Queue.Abstract;
using Soenneker.ServiceBus.Receptor;
using Soenneker.ServiceBus.Receptors.Email.Abstract;

namespace Soenneker.ServiceBus.Receptors.Email;

public sealed class EmailsReceptor : ServiceBusReceptor, IEmailsReceptor
{
    private static readonly MethodInfo _sendMethod = typeof(IEmailSender).GetMethod(nameof(IEmailSender.Send), [typeof(string), typeof(string), typeof(CancellationToken)])!;
    private static readonly object _jobCancellationToken = CancellationToken.None;
    private readonly IBackgroundJobClient? _backgroundJobClient;

    public EmailsReceptor(IServiceBusClientUtil serviceBusClientUtil, IServiceBusQueueUtil serviceBusQueueUtil, ILogger<EmailsReceptor> logger,
        IConfiguration config, IBackgroundJobClient backgroundJobClient) : this(serviceBusClientUtil, serviceBusQueueUtil, logger, config)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public EmailsReceptor(IServiceBusClientUtil serviceBusClientUtil, IServiceBusQueueUtil serviceBusQueueUtil, ILogger<EmailsReceptor> logger, IConfiguration config)
        : base("email", logger, serviceBusClientUtil, serviceBusQueueUtil, config)
    {
    }

    public override ValueTask OnMessageReceived(string messageContent, string type, CancellationToken cancellationToken = default)
    {
        try
        {
            _ = (_backgroundJobClient ?? new BackgroundJobClient()).Create(new Job(typeof(IEmailSender), _sendMethod, [messageContent, type, _jobCancellationToken]), new EnqueuedState());
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Unable to enqueue email job for message type {type}", type);
            throw;
        }

        return ValueTask.CompletedTask;
    }
}
