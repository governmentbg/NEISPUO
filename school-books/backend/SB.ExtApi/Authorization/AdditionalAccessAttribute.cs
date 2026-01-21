namespace SB.ExtApi;

using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class AdditionalAccessAttribute : Attribute
{
    public AdditionalAccessAttribute(string access)
    {
        this.Access = access;
    }

    public string Access { get; private set; }
}
