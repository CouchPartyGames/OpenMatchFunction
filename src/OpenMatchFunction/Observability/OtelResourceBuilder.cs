namespace OpenMatchFunction.Observability;

using OpenTelemetry.Resources;

public static class OtelResourceBuilder
{
    public static ResourceBuilder ResourceBuilder { get; } = ResourceBuilder.CreateDefault()
        .AddService("OpenMatchFunction", null, "1.0.0")
        .AddTelemetrySdk();
}