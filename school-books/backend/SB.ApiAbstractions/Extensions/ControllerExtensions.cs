namespace SB.ApiAbstractions;

using System.Reflection;
using Microsoft.AspNetCore.Hosting;

public static class ControllerExtensions
{
    public static string GetControllerName(this string fullControllerTypeName)
    {
        const string Controller = nameof(Controller);

        if (string.IsNullOrEmpty(fullControllerTypeName) || !fullControllerTypeName.EndsWith(Controller))
            return fullControllerTypeName;

        return fullControllerTypeName[0..^Controller.Length];
    }

    public record VersionResult(string? Version, string Environment);
    public static VersionResult GetVersion(IWebHostEnvironment webHostEnvironment)
    {
        var version = Assembly.GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        return new VersionResult(version, webHostEnvironment.EnvironmentName);
    }
}
