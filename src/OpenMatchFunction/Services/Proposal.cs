namespace OpenMatchFunction.Services;

public interface IProposal
{
    List<Match> GetMatches();
}

public class Proposal : IProposal
{
    public List<Match> GetMatches()
    {
        return [];
    }
}