using System.Diagnostics;

namespace OpenMatchFunction.Observability;

public static class OtelTracing
{
    public static readonly ActivitySource ActivitySource = new(GlobalConsts.ServiceName, GlobalConsts.ServiceVersion);
}