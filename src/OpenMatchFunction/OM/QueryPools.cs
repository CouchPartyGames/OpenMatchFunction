using System.Threading.Tasks;
using System.Threading;
using Google.Protobuf.Collections;

namespace OpenMatchFunction.OM;

public class QueryPools
{
    public async Task<bool> Query(QueryService.QueryServiceClient client, MatchProfile profile)
    {
        var tasks = new List<Task>();
        foreach (var pool in profile.Pools)
        {
           tasks.Add(Task.Run(() =>
           {
               //client.QueryTickets(request);
           })); 
        }
        
        var continuation = Task.WhenAll(tasks);
        
        try
        {
            continuation.Wait();
        }
        catch (AggregateException)
        {
            
        }
        
        return true;
    }
}

public sealed record PoolMap(string PoolName, RepeatedField<Ticket> Tickets);