using Google.Protobuf.Collections;

namespace OpenMatchFunction.Services;

using OpenMatch;
using OpenMatchFunction.OM;

public class MatchFunctionRunService : MatchFunction.MatchFunctionBase
{
	private readonly QueryService.QueryServiceClient _queryClient;

	public MatchFunctionRunService(GrpcClientFactory grpcClientFactory)
	{
		 _queryClient = grpcClientFactory.CreateClient<QueryService.QueryServiceClient>(Constants.OpenMatchQuery);
	}

    public override async Task Run(RunRequest request, IServerStreamWriter<RunResponse> responseStream, ServerCallContext context)
    {
	    /*
	    request.Profile.Name;
	    request.Profile.Pools;
	    request.Profile.Extensions;
	    */

	    QueryTicketsRequest queryRequest = new Query.RequestBuilder()
		    .WithPool(new Pool())
		    .Build();

			// https://openmatch.dev/site/docs/reference/api/#queryservice
	    using var call = _queryClient.QueryTickets(queryRequest);
	    await foreach(var tixResponse in call.ResponseStream.ReadAllAsync())
	    {
				 // Get Proposals (matches)
			var matches = GetMatches(request.Profile, tixResponse.Tickets);
			
			foreach (var match in matches) {
					 // Respond with Proposals
				var response = new Matches.ResponseBuilder()
					.WithMatch(match)
					.Build();

				await responseStream.WriteAsync(response);
			}
	    }
    }

    // https://openmatch.dev/site/docs/reference/api/#openmatch-MatchProfile
    public List<Match> GetMatches(MatchProfile profile, RepeatedField<Ticket> tickets)
    {
	    //profile.Extensions;
	    //profile.Pools;
	    
	    string matchId = "test";
	    string matchFunction = "test";
	    string profileName = profile.Name;
	    
	    var match = new Matches.MatchBuilder()
		    .WithId(matchId)
		    .WithFunctionName(matchFunction)
		    .WithProfileName(profileName)
		    //.AddTicket(tickets)
		    //.AddExtension()
		    .Build();
	    
	    return new List<Match> {
		    match
	    };
    }
}
