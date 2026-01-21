namespace SB.ApiAbstractions;

using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

public class AssumeLocalDateTimeModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (AssumeLocalDateTimeModelBinder.SupportedTypes.Contains(context.Metadata.ModelType))
        {
            return new BinderTypeModelBinder(typeof(AssumeLocalDateTimeModelBinder));
        }

        return null;
    }
}
