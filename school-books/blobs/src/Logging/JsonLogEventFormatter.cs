namespace SB.Blobs;

using System;
using System.Collections.Generic;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

public class JsonLogEventFormatter : ITextFormatter
{
    private const string CommaDelimiter = ",";
    private readonly JsonValueFormatter valueFormatter;
    private readonly HashSet<string> excludedProperties;

    public JsonLogEventFormatter(string[] excludedProperties)
    {
        this.valueFormatter = new JsonValueFormatter(typeTagName: null);
        this.excludedProperties = new HashSet<string>(excludedProperties);
    }

    public void Format(LogEvent logEvent, TextWriter output)
    {
        if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
        if (output == null) throw new ArgumentNullException(nameof(output));

        if (logEvent.Properties.Count != 0)
        {
            output.Write("{");

            var precedingDelimiter = "";
            foreach (var property in logEvent.Properties)
            {
                if(this.excludedProperties.Contains(property.Key))
                {
                    continue;
                }

                output.Write(precedingDelimiter);
                precedingDelimiter = CommaDelimiter;
                JsonValueFormatter.WriteQuotedJsonString(property.Key, output);
                output.Write(':');
                this.valueFormatter.Format(property.Value, output);
            }

            output.Write("}");
        }
        else
        {
            output.Write("{}");
        }
    }
}
