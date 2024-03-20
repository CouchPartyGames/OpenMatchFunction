using System.Diagnostics;

namespace OpenMatchFunction;

public static class Telemetry
{
    public static readonly ActivitySource ActivitySource = new("OpenMatchFunction", "1.0.0");
}