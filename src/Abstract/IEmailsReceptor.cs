using Soenneker.ServiceBus.Receptor.Abstract;

namespace Soenneker.ServiceBus.Receptors.Email.Abstract;

/// <summary>
/// A Hangfire-integrated Service Bus message receptor that deserializes incoming Email messages and enqueues them for webhook processing using a background job.
/// </summary>
public interface IEmailsReceptor : IServiceBusReceptor
{
}