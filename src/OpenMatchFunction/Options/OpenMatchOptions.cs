namespace OpenMatchFunction.Options;

public sealed class OpenMatchOptions
{
    public const string SectionName = "OpenMatch";

    public string QueryHost { get; init; } = "https://open-match-query.open-match.svc.cluster.local:50503";
}