using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Email.Sender.Registrars;
using Soenneker.ServiceBus.Queue.Registrars;
using Soenneker.ServiceBus.Receptors.Email.Abstract;

namespace Soenneker.ServiceBus.Receptors.Email.Registrars;

/// <summary>
/// A Hangfire-integrated Service Bus message receptor that deserializes incoming Email messages and enqueues them for webhook processing using a background job.
/// </summary>
public static class EmailsReceptorRegistrar
{
    /// <summary>
    /// Adds <see cref="IEmailsReceptor"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddEmailsReceptorAsSingleton(this IServiceCollection services)
    {
        services.AddEmailSenderAsScoped().AddServiceBusQueueUtilAsSingleton().TryAddSingleton<IEmailsReceptor, EmailsReceptor>();

        return services;
    }
}