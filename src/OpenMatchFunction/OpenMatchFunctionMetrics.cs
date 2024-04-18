using System.Diagnostics.Metrics;

namespace OpenMatchFunction;

public interface IOpenMatchFunctionMetrics;

public sealed class OpenMatchFunctionMetrics : IOpenMatchFunctionMetrics
{
    private readonly Counter<int> _matchesMade;
    
    public OpenMatchFunctionMetrics(IMeterFactory factory)
    {
        var meter = factory.Create("OpenMatchFunction");

        _matchesMade = meter.CreateCounter<int>("matches.assigned");
    }

    public void MatchesAssigned(int number = 1) => _matchesMade.Add(number);
}