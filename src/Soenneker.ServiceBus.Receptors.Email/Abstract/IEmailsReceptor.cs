using Soenneker.ServiceBus.Receptor.Abstract;

namespace Soenneker.ServiceBus.Receptors.Email.Abstract;

/// <summary>
/// Consumes messages from the <c>email</c> Service Bus queue and enqueues their raw body and type for <c>IEmailSender</c> processing through Hangfire.
/// </summary>
public interface IEmailsReceptor : IServiceBusReceptor
{
}
