using Google.Protobuf.Collections;

namespace OpenMatchFunction.Utilities.OpenMatch;

public record TicketsInPool(string Name, RepeatedField<Ticket> Tickets);

public static class QueryPools
{
    public static async Task<TicketsInPool> QuerySinglePool(QueryService.QueryServiceClient client, 
        Pool pool, 
        CancellationToken token)
    {
        RepeatedField<Ticket> tickets = new();
        QueryTicketsRequest request = new QueryTicketsRequest
        {
            Pool = pool
        };
        
        using var call = client.QueryTickets(request, 
            deadline: DateTime.UtcNow.AddSeconds(5), 
            cancellationToken: token);
        await foreach (var response in call.ResponseStream.ReadAllAsync(token))
        {
            tickets.Add(response.Tickets);
        }

        return new TicketsInPool(pool.Name, tickets);
    }

    public static async Task<List<TicketsInPool>> QueryMultiplePools(QueryService.QueryServiceClient client, 
        RepeatedField<Pool> pools,
        CancellationToken token)
    {
        var tasks = pools.Select(p => QuerySinglePool(client, p, token)).ToArray();
        var results = await Task.WhenAll(tasks);
        return results.Select(r => r).ToList();
        
        /*
        var tasks = new List<Task<TicketsInPool>>();
        foreach (var pool in pools)
        {
            tasks.Add(QuerySinglePool(client, pool, token));
        }

        Task<TicketsInPool[]> continuation = Task.WhenAll(tasks);
        try
        {
            continuation.Wait(token);
        }
        catch (AggregateException) { }
        
        if (continuation.Status == TaskStatus.RanToCompletion)
        {
            return continuation.Result.ToList();
        }
        */

    }
}