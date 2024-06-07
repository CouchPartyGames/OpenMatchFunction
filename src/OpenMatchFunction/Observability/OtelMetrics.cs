using System.Diagnostics.Metrics;

namespace OpenMatchFunction.Observability;

public interface IOpenMatchFunctionMetrics;

public sealed class OtelMetrics : IOpenMatchFunctionMetrics
{
    private readonly Counter<int> _matchesMade;
    
    public OtelMetrics(IMeterFactory factory)
    {
        var meter = factory.Create(GlobalConsts.ServiceName);

        _matchesMade = meter.CreateCounter<int>("matches.assigned");
    }

    public void MatchesAssigned(int number = 1) => _matchesMade.Add(number);
}