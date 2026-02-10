namespace SB.ApiAbstractions;

using NJsonSchema;
using NJsonSchema.Generation;

public class NonNullablePropertiesAsRequiredSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        foreach(var prop in context.Schema.Properties)
        {
            if (!prop.Value.IsNullable(SchemaType.OpenApi3))
            {
                if (!context.Schema.RequiredProperties.Contains(prop.Key))
                {
                    context.Schema.RequiredProperties.Add(prop.Key);
                }
            }
        }
    }
}
