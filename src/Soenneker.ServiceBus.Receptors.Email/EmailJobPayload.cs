namespace Soenneker.ServiceBus.Receptors.Email;

/// <summary>The raw Service Bus body and message type persisted for email delivery.</summary>
public sealed record EmailJobPayload(string MessageContent, string Type);
