namespace SB.Common;

using System;

[AttributeUsage(AttributeTargets.Field)]
public class LocalizationKeyAttribute : Attribute
{
    public string Key { get; }

    public LocalizationKeyAttribute(string key)
    {
        this.Key = key;
    }
}
