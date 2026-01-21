namespace MON.API.Pluralizer
{
    using Humanizer;
    using Microsoft.EntityFrameworkCore.Design;

    public class HumanizerPluralizer : IPluralizer
    {
        public string Pluralize(string identifier)
            => identifier?.Pluralize(inputIsKnownToBeSingular: false);

        public string Singularize(string identifier)
            => identifier?.Singularize(inputIsKnownToBePlural: false);
    }

}
