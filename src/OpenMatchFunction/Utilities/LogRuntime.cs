using System.Runtime.InteropServices;

namespace OpenMatchFunction.Utilities;

public class LogRuntime(ILogger<LogRuntime> logger)
{
    public void LogRuntimeInfo()
    {
        logger.LogInformation("Runtime: {Runtime}", RuntimeInformation.RuntimeIdentifier);
        logger.LogInformation("Framework: {Framework}", RuntimeInformation.FrameworkDescription);
        logger.LogInformation("Process: {Process}", RuntimeInformation.ProcessArchitecture);
        logger.LogInformation("Arch: {Arch}", RuntimeInformation.OSArchitecture);
        logger.LogInformation("OS: {RuntimeInformation.OSDescription}", RuntimeInformation.OSDescription);
    }
}