namespace SB.Cli;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Support;

public partial class CustomEmbeddedScriptProvider : IScriptProvider
{
    [GeneratedRegex("^SB\\.Cli\\.Commands\\.DbUpCommand\\.Scripts\\.(DbScript(\\d+))\\.sql$", RegexOptions.IgnoreCase)]
    private static partial Regex SqlScriptNameRegex();

    private readonly Assembly assembly;

    public CustomEmbeddedScriptProvider()
    {
        this.assembly = Assembly.GetExecutingAssembly();
    }

    public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        =>  from res in this.assembly.GetManifestResourceNames()
            let match = SqlScriptNameRegex().Match(res)
            let name = match.Success ? match.Groups[1].Value : ""
            let number = match.Success ? int.Parse(match.Groups[2].Value) : 0
            where number > 0
            orderby number
            select new SqlScript(
                name,
                this.GetFileContents(this.assembly.GetManifestResourceStream(res)),
                new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 0 });

    private string GetFileContents(Stream file)
    {
        using StreamReader sr = new(file, Encoding.UTF8, true);
        return sr.ReadToEnd();
    }
}
