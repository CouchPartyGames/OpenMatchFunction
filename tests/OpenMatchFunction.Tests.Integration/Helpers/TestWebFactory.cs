namespace OpenMatchFunction.Tests.Integration.Helpers;

public class TestWebFactory : WebApplicationFactory<IApiMarker>
{
    public TestWebFactory()
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseTestServer();
    }
}