namespace OpenMatchFunction.Observability;

using OpenTelemetry.Resources;

public static class OtelResourceBuilder
{
    public static ResourceBuilder ResourceBuilder { get; } = ResourceBuilder.CreateDefault()
        .AddService(GlobalConsts.ServiceName, null, GlobalConsts.ServiceVersion)
        .AddTelemetrySdk();
}