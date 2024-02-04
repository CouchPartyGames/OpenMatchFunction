using System.Threading.Tasks;
using System.Threading;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace OpenMatchFunction.OM;

public record TicketsInPool(string Name, RepeatedField<Ticket> Tickets);

public sealed class QueryPools(QueryService.QueryServiceClient client)
{
    private readonly QueryService.QueryServiceClient _client = client;

    public async Task<TicketsInPool> QuerySinglePool(Pool pool)
    {
        RepeatedField<Ticket> tickets = new();
        QueryTicketsRequest request = new QueryTicketsRequest
        {
            Pool = pool
            
        };
        using var call = _client.QueryTickets(request, deadline: DateTime.UtcNow.AddSeconds(5));
        await foreach (var response in call.ResponseStream.ReadAllAsync())
        {
            tickets.Add(response.Tickets);
        }

        return new TicketsInPool(pool.Name, tickets);
    }

    public List<TicketsInPool> QueryMultiplePools(RepeatedField<Pool> pools)
    {
        var tasks = new List<Task<TicketsInPool>>();
        foreach (var pool in pools)
        {
            tasks.Add(QuerySinglePool(pool));
        }

        Task<TicketsInPool[]> continuation = Task.WhenAll(tasks);
        try
        {
            continuation.Wait();
        }
        catch (AggregateException) { }
        
        if (continuation.Status == TaskStatus.RanToCompletion)
        {
            return continuation.Result.ToList();
        }

        return new List<TicketsInPool>();
    }
}