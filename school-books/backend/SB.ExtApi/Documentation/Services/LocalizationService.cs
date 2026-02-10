namespace SB.ExtApi;

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

public class LocalizationService : ILocalizationService
{
    private readonly Dictionary<string, Dictionary<string, string>> resources = new();

    public LocalizationService(IWebHostEnvironment environment)
    {
        this.LoadAllResources(environment);
    }

    public string GetString(string key, string language = "bg")
    {
        if (this.resources.TryGetValue(language, out var languageResources) &&
            languageResources.TryGetValue(key, out var value))
        {
            return value;
        }
        
        return key;
    }

    private void LoadAllResources(IWebHostEnvironment environment)
    {
        var localizationPath = Path.Combine(environment.ContentRootPath, "Documentation", "Languages");
        
        if (!Directory.Exists(localizationPath))
        {
            throw new DirectoryNotFoundException($"Localization directory not found: {localizationPath}");
        }

        foreach (var filePath in Directory.GetFiles(localizationPath, "*.json"))
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip, // Ignore comments in JSON
                AllowTrailingCommas = true
            };
            var languageResources = JsonSerializer.Deserialize<Dictionary<string, string>>(json, options) 
                                   ?? new Dictionary<string, string>();

            this.resources[fileName] = languageResources;
        }
    }
}
