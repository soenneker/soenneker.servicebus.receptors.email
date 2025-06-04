using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Email.Senders.Abstract;
using Soenneker.ServiceBus.Client.Abstract;
using Soenneker.ServiceBus.Queue.Abstract;
using Soenneker.ServiceBus.Receptor;
using Soenneker.ServiceBus.Receptors.Email.Abstract;

namespace Soenneker.ServiceBus.Receptors.Email;

///<inheritdoc cref="IEmailsReceptor"/>
public sealed class EmailsReceptor : ServiceBusReceptor, IEmailsReceptor
{
    public EmailsReceptor(IServiceBusClientUtil serviceBusClientUtil, IServiceBusQueueUtil serviceBusQueueUtil, ILogger<EmailsReceptor> logger, IConfiguration config)
        : base("email", logger, serviceBusClientUtil, serviceBusQueueUtil, config)
    {
    }

    public override ValueTask OnMessageReceived(string messageContent, Type? type, CancellationToken cancellationToken = default)
    {
        try
        {
            _ = BackgroundJob.Enqueue<IEmailSender>(x => x.Send(messageContent, type, CancellationToken.None));
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Unable to queue job with content: {content}", messageContent);
        }

        return ValueTask.CompletedTask;
    }
}