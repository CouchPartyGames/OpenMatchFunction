using System.Diagnostics;

namespace OpenMatchFunction.Observability;

public static class OtelTracing
{
    public static readonly ActivitySource ActivitySource = new("OpenMatchFunction", "1.0.0");
}