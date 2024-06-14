using Grpc.Core;
using NSubstitute.ReturnsExtensions;
using OpenMatch;
using OpenMatchFunction.Services;
using OpenMatchFunction.Tests.Unit.Helpers;

namespace OpenMatchFunction.Tests.Unit;

public class MatchFunctionRunServiceTests
{
    private readonly IMatchFunctionRunService _sut;
    
    public MatchFunctionRunServiceTests()
    {
        var mockClient = Substitute.For<QueryService.QueryServiceClient>("hello");
        //mockClient.QueryTickets().ResponseStream.ReadAllAsync().;
        
        /*
        mockClient
            .Setup(m => m.SayHelloUnaryAsync(
                It.IsAny<HelloRequest>(), null, null, CancellationToken.None))
            .Returns(mockCall);
            */
        
        //var metrics = Substitute.For<IOpenMatchFunctionMetrics>();
        //_sut = new MatchFunctionRunService(mockClient, metrics);
    }
    
    [Fact]
    public void Test1()
    {
        // Arrange
        var request = new RunRequest();
        var callContext = new TestServerCallContext(new Metadata(), new CancellationToken());
        var expectedResponse = new RunResponse();
        
        // Act
        //var actualResponse = _sut.Run();
            
        // Assert
        //actualResponse.Should().BeEquivalentTo(expectedResponse);
    }
    
}