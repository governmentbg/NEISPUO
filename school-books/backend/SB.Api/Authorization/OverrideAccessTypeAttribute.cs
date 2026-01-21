namespace SB.Api;

using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class OverrideAccessTypeAttribute : Attribute
{
    public OverrideAccessTypeAttribute(AccessType accessType)
    {
        this.AccessType = accessType;
    }

    public AccessType AccessType { get; private set; }
}
