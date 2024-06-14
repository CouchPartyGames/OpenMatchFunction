namespace OpenMatchFunction.Services;

public interface IProposal
{
    List<Ticket> GetMatches();
    //List<Match> GetMatches();
}



public class SimpleProposal : IProposal
{
    public List<Ticket> GetMatches()
    {
        return [];
    }
}

public class ComplexProposal : IProposal
{
    public List<Ticket> GetMatches()
    {
        return [];
    }
}