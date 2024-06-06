namespace OpenMatchFunction.Utilities.OpenMatch;

public sealed class Matches
{
    
    public class ResponseBuilder
    {
        private Match match = new();

        public ResponseBuilder WithMatch(Match match) {
            this.match = match;
            return this;
        }

        public RunResponse Build()
        {
            return new RunResponse
            {
                Proposal = match
            };
        }
    }
    
    public sealed class MatchBuilder
    {
        private Match _match = new();

        public MatchBuilder WithFunctionName(string name)
        {
            _match.MatchFunction = name;
            return this;
        }
        public MatchBuilder WithId(string id)
        {
            _match.MatchId = id;
            return this;
        }

        public MatchBuilder WithProfileName(string name)
        {
            _match.MatchProfile = name;
            return this;
        }

        public MatchBuilder AddTicket(Ticket ticket)
        {
            _match.Tickets.Add(ticket);
            return this;
        }

        public MatchBuilder AddTickets(List<Ticket> tickets)
        {
            _match.Tickets.Add(tickets);
            return this;
        }

        public MatchBuilder AddExtension()
        {
            return this;
        }
        
        public Match Build() => _match;
    }
}