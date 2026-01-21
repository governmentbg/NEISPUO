namespace SB.ApiAbstractions;

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class AssumeLocalDateTimeModelBinder : IModelBinder
{
    public static readonly Type[] SupportedTypes = new [] { typeof(DateTime), typeof(DateTime?) };

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        if (!SupportedTypes.Contains(bindingContext.ModelType))
        {
            return Task.CompletedTask;
        }

        var modelName = this.GetModelName(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var dateToParse = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(dateToParse))
        {
            return Task.CompletedTask;
        }

        var dateTime = this.ParseDate(dateToParse);

        bindingContext.Result = ModelBindingResult.Success(dateTime);

        return Task.CompletedTask;
    }

    private DateTime? ParseDate(string dateToParse)
    {
        if (DateTime.TryParse(dateToParse, null, DateTimeStyles.AssumeLocal, out var validDate))
        {
            return validDate;
        }

        return null;
    }

    private string GetModelName(ModelBindingContext bindingContext)
    {
        // The "Name" property of the ModelBinder attribute can be used to specify the
        // route parameter name when the action parameter name is different from the route parameter name.
        // For instance, when the route is /api/{birthDate} and the action parameter name is "date".
        // We can add this attribute with a Name property [DateTimeModelBinder(Name ="birthDate")]
        // Now bindingContext.BinderModelName will be "birthDate" and bindingContext.ModelName will be "date"
        if (!string.IsNullOrEmpty(bindingContext.BinderModelName))
        {
            return bindingContext.BinderModelName;
        }

        return bindingContext.ModelName;
    }
}
