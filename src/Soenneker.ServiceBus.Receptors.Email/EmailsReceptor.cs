using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Flywheel.Core.Services.Abstract;
using Soenneker.ServiceBus.Client.Abstract;
using Soenneker.ServiceBus.Queue.Abstract;
using Soenneker.ServiceBus.Receptor;
using Soenneker.ServiceBus.Receptors.Email.Abstract;

namespace Soenneker.ServiceBus.Receptors.Email;

public sealed class EmailsReceptor : ServiceBusReceptor, IEmailsReceptor
{
    private readonly IJobClient _jobs;

    public EmailsReceptor(IServiceBusClientUtil serviceBusClientUtil, IServiceBusQueueUtil serviceBusQueueUtil, ILogger<EmailsReceptor> logger,
        IConfiguration config, IJobClient jobs) : base("email", logger, serviceBusClientUtil, serviceBusQueueUtil, config)
    {
        _jobs = jobs;
    }

    public override async ValueTask OnMessageReceived(string messageContent, string type, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jobs.Enqueue(EmailJobInvoker.Definition, new EmailJobPayload(messageContent, type), cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Unable to enqueue email job for message type {type}", type);
            throw;
        }
    }
}
