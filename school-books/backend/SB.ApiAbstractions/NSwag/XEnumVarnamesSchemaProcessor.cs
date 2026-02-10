namespace SB.ApiAbstractions;

using System.Collections.Generic;
using System.Linq;
using NJsonSchema.Generation;

public class XEnumVarnamesSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        if (context.ContextualType.Type.IsEnum && context.Schema.EnumerationNames.Count > 0)
        {
            if (context.Schema.ExtensionData is null)
            {
                context.Schema.ExtensionData = new Dictionary<string, object>();
            }

            if (!context.Schema.ExtensionData.ContainsKey("x-enum-varnames"))
            {
                context.Schema.ExtensionData.Add("x-enum-varnames", context.Schema.EnumerationNames.ToArray());
            }
        }
    }
}
