namespace SB.ApiAbstractions;

using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

public class TextPlainAsStringOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        if (context
            .OperationDescription
            .Operation
            .RequestBody
            ?.Content
            ?.TryGetValue("text/plain", out var textPlainMediaType) == true)
        {
            textPlainMediaType.Schema.Format = null;
        }

        return true;
    }
}
