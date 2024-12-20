namespace OpenMatchFunction.Observability;

public static class OtelTracing
{
    public static readonly ActivitySource ActivitySource = new(GlobalConstants.ServiceName, GlobalConstants.ServiceVersion);

    public const string RunRequest = "RunRequest";
    public const string FetchTickets = "FetchTickets";
    public const string GetMatches = "GetMatches";
    public const string ReturnMatches = "ReturnMatches";
}