namespace SB.Cli;

using System.Threading;
using Microsoft.Extensions.CommandLineUtils;

public interface ICommand
{
    string Name { get; }

    void Configure(CommandLineApplication app, CancellationToken stopped);
}
