namespace SB.ApiAbstractions;

using NJsonSchema;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

public class NonNullableParametersAsRequiredOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        foreach(var p in context.Parameters)
        {
            if (!p.Value.IsNullable(SchemaType.OpenApi3))
            {
                if (!p.Value.IsRequired)
                {
                    p.Value.IsRequired = true;
                }
            }
        }

        return true;
    }
}
