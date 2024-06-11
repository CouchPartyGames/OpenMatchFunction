namespace OpenMatchFunction.Options;

public sealed class OpenMatchOptions
{
    public const string SectionName = "OpenMatch";
    
    public const string OpenMatchQuery = "OpenMatchQuery";
    
    public const string OpenMatchQueryDefaultHost = "https://open-match-query.open-match.svc.cluster.local:50503";

    public string QueryHost { get; init; } = OpenMatchQueryDefaultHost;
}