namespace OpenMatchFunction.Observability;

using OpenTelemetry.Resources;

public static class OtelResourceBuilder
{
    public static ResourceBuilder ResourceBuilder { get; } = ResourceBuilder.CreateDefault()
        .AddService(GlobalConstants.ServiceName, null, GlobalConstants.ServiceVersion)
        .AddTelemetrySdk();
}