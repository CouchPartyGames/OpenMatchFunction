namespace OpenMatchFunction.Options;

public sealed class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetry";

    public bool Enabled { get; init; } = false;

    public string EndpointType { get; init; } = "otlp";
    
    public string Endpoint { get; init; }
}