namespace SB.ExtApi;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

public class MultiLanguageProcessor : IOperationProcessor, ISchemaProcessor
{
    private readonly string language;
    private readonly ILocalizationService? localizationService;
    private readonly string pattern = @"\{\{([^}]*)\}\}";

    public MultiLanguageProcessor(string language, ILocalizationService localizationService)
    {
        this.language = language;
        this.localizationService = localizationService;
    }

    public MultiLanguageProcessor(string language = "bg")
    {
        this.language = language;
        this.localizationService = null;
    }

    public bool Process(OperationProcessorContext context)
    {
        var operation = context.OperationDescription.Operation;

        if (this.localizationService != null)
        {
            if (!string.IsNullOrEmpty(operation.Summary))
            {
                operation.Summary = this.ProcessContent(operation.Summary.Trim(), this.localizationService);
            }

            if (!string.IsNullOrEmpty(operation.Description))
            {
                operation.Description = this.ProcessContent(operation.Description.Trim(), this.localizationService);
            }

            foreach (var parameter in operation.Parameters)
            {
                if (!string.IsNullOrEmpty(parameter.Description))
                {
                    parameter.Description = this.ProcessContent(parameter.Description.Trim(), this.localizationService);
                }
            }

            foreach (var response in operation.Responses)
            {
                if (!string.IsNullOrEmpty(response.Value.Description))
                {
                    response.Value.Description = this.ProcessContent(response.Value.Description, this.localizationService);
                }
            }
        }

        return true;
    }

    public void Process(SchemaProcessorContext context)
    {
        if (this.localizationService == null)
            return;

        var schema = context.Schema;

        if (!string.IsNullOrEmpty(schema.Description))
        {
            schema.Description = this.ProcessContent(schema.Description.Trim(), this.localizationService);
        }

        if (!string.IsNullOrEmpty(schema.Title))
        {
            schema.Title = this.ProcessContent(schema.Title.Trim(), this.localizationService);
        }

        if (context.ContextualType?.Type != null && context.ContextualType.Type.IsEnum)
        {
            this.LocalizeEnumDescriptions(schema, context.ContextualType.Type);
        }

        if (schema.Properties != null)
        {
            foreach (var property in schema.Properties.Values)
            {
                if (!string.IsNullOrEmpty(property.Description))
                {
                    property.Description = this.ProcessContent(property.Description.Trim(), this.localizationService);
                }

                if (!string.IsNullOrEmpty(property.Title))
                {
                    property.Title = this.ProcessContent(property.Title.Trim(), this.localizationService);
                }
            }
        }
    }

    public static bool IsLanguageSupported(string language)
    {
        return !string.IsNullOrEmpty(language) && Enum.TryParse<SupportedLanguages>(language.ToUpper(), out _ );
    }

    private void LocalizeEnumDescriptions(JsonSchema schema, Type enumType)
    {
        if (schema.EnumerationNames == null || schema.EnumerationNames.Count == 0)
            return;

        var enumNames = Enum.GetNames(enumType);

        for (int i = 0; i < schema.EnumerationNames.Count && i < enumNames.Length; i++)
        {
            var enumName = enumNames[i];
            var localizationKey = $"{enumType.Name}.{enumName}";
            var localizedDescription = this.localizationService?.GetString(localizationKey, this.language);

            if (!string.IsNullOrEmpty(localizedDescription) && localizedDescription != localizationKey)
            {
                schema.EnumerationNames[i] = localizedDescription;
            }
        }

        if (schema.ExtensionData?.ContainsKey("x-enum-descriptions") == true)
        {
            schema.ExtensionData["x-enum-descriptions"] = schema.EnumerationNames.ToArray();
        }
    }

    private string ProcessContent(string content, ILocalizationService localizationService)
    {
        var result = Regex.Replace(content, this.pattern, match =>
        {
            if (match.Groups.Count > 1 && match.Groups[1].Success)
            {
                var key = match.Groups[1].Value.Trim();
                var localizedValue = localizationService.GetString(key, this.language);

                return localizedValue != key ? localizedValue : match.Value;
            }
            return match.Value;
        });

        return this.ExtractLanguageContent(result, this.language);
    }

    private string ExtractLanguageContent(string content, string language)
    {
        var lines = content.Split('\n');

        foreach (var line in lines)
        {
            if (line.Trim().StartsWith($"{language.ToUpper()}:", StringComparison.OrdinalIgnoreCase))
            {
                return line.Substring(line.IndexOf(':') + 1).Trim();
            }
        }
        return content;
    }
}

public enum SupportedLanguages
{
    BG,
    EN
}
