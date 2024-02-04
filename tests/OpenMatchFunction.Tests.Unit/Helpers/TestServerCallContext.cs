using Grpc.Core;

namespace OpenMatchFunction.Tests.Unit.Helpers;

public class TestServerCallContext : ServerCallContext
{
    private readonly Metadata _metadata;
    private readonly CancellationToken _cancellationToken;

    private TestServerCallContext(Metadata metadata, CancellationToken cancellationToken)
    {
        _metadata = metadata;
        _cancellationToken = cancellationToken;
    }
    
    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
    {
        throw new NotImplementedException();
    }

    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options)
    {
        throw new NotImplementedException();
    }

    protected override string MethodCore { get; } = "MethodDefault";
    protected override string HostCore { get; } = "HostName";
    protected override string PeerCore { get; } = "PeerName";
    protected override DateTime DeadlineCore { get; }
    protected override Metadata RequestHeadersCore { get; }
    protected override CancellationToken CancellationTokenCore { get; }
    protected override Metadata ResponseTrailersCore { get; }
    protected override Status StatusCore { get; set; }
    protected override WriteOptions? WriteOptionsCore { get; set; }
    protected override AuthContext AuthContextCore { get; }
}