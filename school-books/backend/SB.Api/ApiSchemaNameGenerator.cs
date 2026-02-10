namespace SB.Api;

using NJsonSchema.Generation;
using System;
using System.Text.RegularExpressions;

internal partial class ApiSchemaNameGenerator : DefaultSchemaNameGenerator
{
    [GeneratedRegex("^I(?<typeName>.+)QueryRepository$")]
    private static partial Regex IQueryRepositoryRegex();

    public override string Generate(Type type) =>
        type.DeclaringType != null ?
            TrimIQueryRepository(type.DeclaringType.Name) + base.Generate(type) :
            base.Generate(type);

    private static string TrimIQueryRepository(string name) =>
        IQueryRepositoryRegex().IsMatch(name) ?
            IQueryRepositoryRegex().Match(name).Groups["typeName"].Value :
            name;
}
