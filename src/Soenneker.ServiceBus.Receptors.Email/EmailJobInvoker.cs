using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Email.Senders.Abstract;
using Soenneker.Flywheel.Communication.Dtos;
using Soenneker.Flywheel.Core.Services.Abstract;

namespace Soenneker.ServiceBus.Receptors.Email;

internal sealed class EmailJobInvoker : IJobInvoker
{
    internal static readonly JobDefinition<EmailJobPayload> Definition = new("soenneker.email.send.v1");

    public string Name => Definition.Name;

    public async Task Invoke(IServiceProvider services, string payload, CancellationToken cancellationToken)
    {
        EmailJobPayload message = JsonSerializer.Deserialize(payload, EmailJobJsonContext.Default.EmailJobPayload)
            ?? throw new InvalidOperationException("An email job payload is required.");
        await services.GetRequiredService<IEmailSender>().Send(message.MessageContent, message.Type, cancellationToken);
    }
}
