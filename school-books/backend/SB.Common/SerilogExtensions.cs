namespace SB.Common;

using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System.Linq;

public static class SerilogExtensions
{
    // A fix for dotnet running in a Linux container max env var name length limitation
    // see https://github.com/serilog/serilog-settings-configuration/issues/214#issuecomment-605684024
    public static LoggerConfiguration ApplyCustomOverrides(this LoggerConfiguration cfg, IConfigurationSection section)
        => section.GetChildren().Aggregate(cfg, (c, s) => c.MinimumLevel.Override(s.GetValue<string>("SourceContext")!, s.GetValue<LogEventLevel>("Level")));
}
