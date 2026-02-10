namespace SB.Blobs;

using System;
using Serilog.Core;
using Serilog.Events;

public class ElapsedEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.TryGetValue("Elapsed", out var value) &&
            value is ScalarValue { Value: double doubleElapsed })
        {
            logEvent.RemovePropertyIfPresent("Elapsed");
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SB_Elapsed", Convert.ToInt32(doubleElapsed)));
        }
    }
}
