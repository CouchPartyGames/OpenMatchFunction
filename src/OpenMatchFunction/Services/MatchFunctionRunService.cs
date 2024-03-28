namespace OpenMatchFunction.Services;

using OpenMatch;
using OpenMatchFunction.OM;

public interface IMatchFunctionRunService;

public class MatchFunctionRunService : MatchFunction.MatchFunctionBase, IMatchFunctionRunService
{
	private readonly QueryService.QueryServiceClient _queryClient;

	private const string MatchFunctionName = "basic-match";

	private QueryPools _queryPools;

	public MatchFunctionRunService(GrpcClientFactory grpcClientFactory)
	{
		 _queryClient = grpcClientFactory.CreateClient<QueryService.QueryServiceClient>(Constants.OpenMatchQuery);
		 _queryPools = new QueryPools(_queryClient);
	}

    public override async Task Run(RunRequest request, IServerStreamWriter<RunResponse> responseStream, ServerCallContext context)
    {
	    List<TicketsInPool> tickets = new();
	    using (var activity = Telemetry.ActivitySource.StartActivity("RunRequest"))
	    {
		    var shouldThrow = false;
		    if (shouldThrow)
		    {
			    throw new RpcException(new Status(StatusCode.InvalidArgument, "Name is required."));
		    }

		    using (Telemetry.ActivitySource.StartActivity("FetchTickets"))
		    {
			    // Fetch Tickets from Pools
			    tickets = _queryPools.QueryMultiplePools(request.Profile.Pools);
		    }

		    // Generate Proposals
		    var proposals = GetProposals(request.Profile, tickets);

		    // Send Back Matches
		    foreach (var match in proposals)
		    {
			    // Respond with Proposals
			    var response = new Matches.ResponseBuilder()
				    .WithMatch(match)
				    .Build();

			    await responseStream.WriteAsync(response);
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
