using OpenMatchFunction.Utilities.OpenMatch;

namespace OpenMatchFunction.Services;

public static class ProposalFactory
{
	public enum ProposalType
	{
		Simple = 0,
		Complex
	}

	public static AbstractProposal NewProposal(ProposalType @type, 
		MatchProfile profile,
		List<TicketsInPool> ticketsInPools) => type switch
	{
		ProposalType.Simple => new SimpleProposal(profile, ticketsInPools),
		_ => new ComplexProposal(profile, ticketsInPools)
	};
}

public abstract class AbstractProposal
{
	public string MatchId { get; protected set; } = "profile-basic";
	public string MatchFunction { get; protected set; } = "basic-match";
	public string ProfileName { get; protected set; } = "basic";

	protected readonly MatchProfile _profile;
	protected readonly List<TicketsInPool> _ticketsInPools;
	
	public AbstractProposal(MatchProfile profile, List<TicketsInPool> ticketsInPools)
	{
		_profile = profile;
		_ticketsInPools = ticketsInPools;
	}

	protected Match GetMatch(List<Ticket> tickets)
	{
		return new Matches.MatchBuilder()
			.WithId(MatchId)
			.WithFunctionName(MatchFunction)
			.WithProfileName(ProfileName)
			.AddTickets(tickets)
			.Build();
	}
}

public class SimpleProposal : AbstractProposal
{
	public SimpleProposal(MatchProfile profile, List<TicketsInPool> ticketsInPools) : base(profile, ticketsInPools)
	{
	}
	
	public List<Match> GetMatches()
    {
	    var matches = new List<Match>();
	    var matchTickets = new List<Ticket>();
	    
		foreach (var item in _ticketsInPools)
		{
			 foreach (var tix in item.Tickets.ToList())
			 {
				 matchTickets.Add(tix);
			 }
			 // Remove from Pool Tickets
		}

		matches.Add(GetMatch(matchTickets));
		
        return matches;
    }

}

public class ComplexProposal(MatchProfile profile, List<TicketsInPool> ticketsInPools)
	: AbstractProposal(profile, ticketsInPools)
{
    public List<Ticket> GetMatches()
    {
        return [];
    }
}