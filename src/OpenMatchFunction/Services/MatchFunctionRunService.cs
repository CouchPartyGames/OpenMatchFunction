using Google.Rpc;
using OpenMatchFunction.Options;
using OpenMatchFunction.Utilities.OpenMatch;
using Status = Grpc.Core.Status;

namespace OpenMatchFunction.Services;

using OpenMatch;
using OpenMatchFunction.Observability;

public interface IMatchFunctionRunService;

public class MatchFunctionRunService : MatchFunction.MatchFunctionBase, IMatchFunctionRunService
{
	private readonly QueryService.QueryServiceClient _queryClient;
	private readonly OtelMetrics _metrics;

	private const string MatchFunctionName = "basic-match";

	private readonly QueryPools _queryPools;

	private static readonly Google.Rpc.Status QueryError = new Google.Rpc.Status
	{
		Code	= (int)Code.FailedPrecondition,
		Message = "Failed Query Pools",
		Details = {}
	};
	
	private static readonly Google.Rpc.Status ProposalError = new Google.Rpc.Status
	{
		Code	= (int)Code.FailedPrecondition,
		Message = "Failed Match Proposals",
		Details = {}
	};

	public MatchFunctionRunService(GrpcClientFactory grpcClientFactory, 
		OtelMetrics metrics)
	{
		_metrics = metrics;
		_queryClient = grpcClientFactory.CreateClient<QueryService.QueryServiceClient>(OpenMatchOptions.OpenMatchQuery); 
		_queryPools = new QueryPools(_queryClient, new CancellationToken());
	}

    public override async Task Run(RunRequest request, IServerStreamWriter<RunResponse> responseStream, ServerCallContext context)
    {
	    using (var activity = OtelTracing.ActivitySource.StartActivity("RunRequest"))
	    {
		    Console.WriteLine(request);
		    var shouldThrow = false;
		    if (shouldThrow)
		    {
			    throw new RpcException(new Status(StatusCode.InvalidArgument, "Name is required."));
		    }

			List<TicketsInPool> tickets = [];
		    using (OtelTracing.ActivitySource.StartActivity("FetchTickets"))
		    {
			    // Fetch Tickets from Pools
			    tickets = _queryPools.QueryMultiplePools(request.Profile.Pools);
			    
		    }
		    if (tickets.Count == 0)
		    {
			    throw QueryError.ToRpcException();
		    }

		    // Generate Proposals
		    var proposals = GetProposals(request.Profile, tickets);
		    if (proposals.Count == 0)
		    {
			    throw ProposalError.ToRpcException();
		    }

		    // Send Back All Matches
		    foreach(Match m in proposals)
		    {
			    // Add Logging, Metrics
			    await responseStream.WriteAsync(new Matches.ResponseBuilder()
				    .WithMatch(m)
				    .Build());
		    }
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

		var match = new Matches.MatchBuilder()
			 .WithId(matchId)
			 .WithFunctionName(matchFunction)
			 .WithProfileName(profileName)
			 .AddTickets(matchTickets)
			 //.AddExtension()
			 .Build();

		matches.Add(match);
	    return matches;
    }
}
