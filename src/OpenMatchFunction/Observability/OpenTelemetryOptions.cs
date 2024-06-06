namespace OpenMatchFunction.Options;

public sealed class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetry";

    public const string OtelDefaultHost = "http://localhost:4317";
    

    
    public bool Enabled { get; init; } = false;

    public string EndpointType { get; init; } = "otlp";

    // OpenTelemetry rate to sample traces
    public float SamplingRate { get; init; } = 1.0f;

    public string Endpoint { get; init; } = OtelDefaultHost;
}