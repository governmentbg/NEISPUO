namespace SB.Data;

using System;
using System.Text.Json.Serialization;
using SB.Common;

public class EnumNomVO<TEnum> : IEquatable<EnumNomVO<TEnum>>
    where TEnum : struct, IConvertible
{
    private TEnum e;

    public EnumNomVO(TEnum e)
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }

        this.e = e;
    }

    public TEnum Id
    {
        get { return this.e; }
    }

    [JsonConverter(typeof(EnumDescriptionConverter))]
    public TEnum Name
    {
        get { return this.e; }
    }

    public int OrderNum
    {
        get { return Convert.ToInt32(this.e); }
    }

    public override int GetHashCode()
    {
        return this.e.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }

        return this.Equals((EnumNomVO<TEnum>)obj);
    }

    public bool Equals(EnumNomVO<TEnum>? other)
    {
        return other?.e.Equals(this.e) ?? false;
    }
}
