using System.Text.Json.Serialization;

namespace Soenneker.ServiceBus.Receptors.Email;

/// <summary>JSON metadata for email job payloads. Include this contract in the host's Flywheel payload serializer.</summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(EmailJobPayload))]
public partial class EmailJobJsonContext : JsonSerializerContext;
