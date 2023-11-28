namespace OpenMatchFunction.OM;

public class Query
{

    public class RequestBuilder
    {
        private QueryTicketsRequest _request = new();

        public RequestBuilder WithPools(Pool pool)
        {
            _request.Pool = pool;
            return this;
        }
        
        public QueryTicketsRequest Build() => _request;
    }
}