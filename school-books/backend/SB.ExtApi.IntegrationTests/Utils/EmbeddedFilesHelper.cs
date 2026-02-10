namespace SB.ExtApi.IntegrationTests;

using System.IO;
using System.Reflection;

static class EmbeddedFilesHelper
{
    private static Assembly IntegrationTestsAssembly = typeof(EmbeddedFilesHelper).Assembly;
    private static Stream CreateEmbeddedFileReadStream(Assembly assembly, string subpath)
        => assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{subpath.Replace("/", ".")}")
            ?? throw new FileNotFoundException($"Embedded file '{subpath}' not found in assembly '{assembly.FullName}'.");

    public static Stream CreateEmbeddedFileReadStream(string subpath)
        => CreateEmbeddedFileReadStream(IntegrationTestsAssembly, subpath);
}
