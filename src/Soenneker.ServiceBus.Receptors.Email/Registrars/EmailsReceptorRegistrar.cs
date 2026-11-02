using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.ServiceBus.Queue.Registrars;
using Soenneker.Flywheel.Core.Services.Abstract;
using Soenneker.ServiceBus.Receptors.Email.Abstract;

namespace Soenneker.ServiceBus.Receptors.Email.Registrars;

/// <summary>
/// Registers the Flywheel-backed <c>email</c> queue receptor and its Service Bus dependencies.
/// </summary>
public static class EmailsReceptorRegistrar
{
    /// <summary>
    /// Adds <see cref="IEmailsReceptor"/> as a singleton service. <para/>
    /// </summary>
    /// <remarks>The host must configure Flywheel and its storage provider, register IEmailSender, and include EmailJobPayload in its Flywheel JSON context.</remarks>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddEmailsReceptorAsSingleton(this IServiceCollection services)
    {
        services.AddServiceBusQueueUtilAsSingleton().TryAddSingleton<IEmailsReceptor, EmailsReceptor>();

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IJobInvoker, EmailJobInvoker>());

        return services;
    }
}
