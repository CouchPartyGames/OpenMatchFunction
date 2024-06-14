namespace OpenMatchFunction.Services;

public sealed class ServiceOptions
{ 
    public const string SectionName = "Service";

    public int MaxGrpcReceiveSize { get; init; } = 1024 * 1024 * 4;

    public int MaxGrpcSendSize { get; init; } = 1024 * 1024 * 4;
}