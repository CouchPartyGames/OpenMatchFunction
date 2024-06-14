using FluentValidation.Results;
using OpenMatchFunction.Clients.OpenMatchPool.Options;
using OpenMatchFunction.Interceptors;
using OpenMatchFunction.Utilities.OpenMatch;

namespace OpenMatchFunction.Services;

using OpenMatch;
using OpenMatchFunction.Observability;

public interface IMatchFunctionRunService;

public class MatchFunctionRunService(
	GrpcClientFactory grpcClientFactory,
	ILogger<MatchFunctionRunService> logger,
	OtelMetrics metrics)
	: MatchFunction.MatchFunctionBase, IMatchFunctionRunService
{
	private readonly QueryService.QueryServiceClient _queryClient = grpcClientFactory.CreateClient<QueryService.QueryServiceClient>(OpenMatchOptions.OpenMatchQuery);
	private readonly OtelMetrics _metrics = metrics;

	private const string MatchFunctionName = "basic-match";
	private readonly CancellationToken _token = new CancellationToken();

	public override async Task Run(RunRequest request, IServerStreamWriter<RunResponse> responseStream, ServerCallContext context)
    {
	    using var activity = OtelTracing.ActivitySource.StartActivity("RunRequest");
	    
	    ValidateRunRequest(request);

	    MatchFunctionRunServiceLog.LogStage(logger, "QueryMultiplePools");
	    List<TicketsInPool> tickets = [];
	    using (OtelTracing.ActivitySource.StartActivity("FetchTickets"))
	    {
		    // Fetch Tickets from Pools
		    tickets = await QueryPools.QueryMultiplePools(_queryClient, request.Profile.Pools, _token);
			if (tickets.Count == 0)
			{
				throw ServiceErrors.QueryError.ToRpcException();
			}
	    }

	    // Generate Proposals
	    MatchFunctionRunServiceLog.LogStage(logger, "GenerateProposals");
	    var proposals = GetProposals(request.Profile, tickets);
	    if (proposals.Count == 0)
	    {
		    throw ServiceErrors.ProposalError.ToRpcException();
	    }

	    // Send Back All Matches
	    MatchFunctionRunServiceLog.LogStage(logger, "GenerateProposals");
	    foreach (Match m in proposals)
	    {
		    // Add Logging, Metrics
		    await responseStream.WriteAsync(new Matches.ResponseBuilder()
			    .WithMatch(m)
			    .Build());
	    }
    }

    // https://openmatch.dev/site/docs/reference/api/#openmatch-MatchProfile
    public List<Match> GetProposals(MatchProfile profile, List<TicketsInPool> tickets)
    {
	    var matches = new List<Match>();
	    var matchTickets = new List<Ticket>();
		string matchFunction = MatchFunctionName;
		string matchId = $"profile-{profile.Name}-";
		string profileName = profile.Name;
	    
		foreach (var item in tickets)
		{
			 foreach (var tix in item.Tickets.ToList())
			 {
				 matchTickets.Add(tix);
			 }
			 // Remove from Pool Tickets
		}

		Match match = new Matches.MatchBuilder()
			 .WithId(matchId)
			 .WithFunctionName(matchFunction)
			 .WithProfileName(profileName)
			 .AddTickets(matchTickets)
			 //.AddExtension()
			 .Build();

		matches.Add(match);
	    return matches;
    }

    private void ValidateRunRequest(RunRequest request)
    {
	    RunRequestValidator validator = new();
	    ValidationResult results = validator.Validate(request);
	    if (!results.IsValid)
	    {
		    throw ServiceErrors.ValidationError.ToRpcException();
	    }
    }
}
public static partial class MatchFunctionRunServiceLog
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Entering Stage: {Stage}")]
    public static partial void LogStage(
        ILogger logger, string Stage);
}
