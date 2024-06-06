namespace OpenMatchFunction.Utilities.OpenMatch;

public class Query
{

    public class RequestBuilder
    {
        private QueryTicketsRequest _request = new();

        public RequestBuilder WithPool(Pool pool)
        {
            _request.Pool = pool;
            return this;
        }
        
        public QueryTicketsRequest Build() => _request;
    }
}