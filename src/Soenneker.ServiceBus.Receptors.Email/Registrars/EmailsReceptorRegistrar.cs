using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddEmailsReceptorAsSingleton(this IServiceCollection services)
    {
        services.AddServiceBusQueueUtilAsSingleton().TryAddSingleton<IEmailsReceptor, EmailsReceptor>();

        return services;
    }
}
