namespace SB.ExtApi;

public interface ILocalizationService
{
    string GetString(string key, string language = "bg");
}
