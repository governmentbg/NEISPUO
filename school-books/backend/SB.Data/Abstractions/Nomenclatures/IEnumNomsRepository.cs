namespace SB.Data;

using System;
using System.Collections.Generic;

public interface IEnumNomsRepository<TEnum>
    where TEnum : struct, IConvertible
{
    IList<EnumNomVO<TEnum>> GetNomsById(TEnum[] enums);

    IList<EnumNomVO<TEnum>> GetNomsByTerm(string? term);
}
