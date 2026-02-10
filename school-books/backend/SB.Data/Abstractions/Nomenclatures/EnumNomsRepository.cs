namespace SB.Data;

using SB.Common;
using System;
using System.Collections.Generic;
using System.Linq;

class EnumNomsRepository<TEnum> : IEnumNomsRepository<TEnum>
    where TEnum : struct, IConvertible
{
    public EnumNomsRepository()
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }
    }

    public IList<EnumNomVO<TEnum>> GetNomsById(TEnum[] enums)
    {
        if (!enums.Any())
        {
            throw new ArgumentException($"'{nameof(enums)}' should be non-empty.");
        }

        return enums
            .Select(e => new EnumNomVO<TEnum>(e))
            .ToList();
    }

    public virtual IList<EnumNomVO<TEnum>> GetNomsByTerm(string? term)
    {
        Func<string?, bool> termFilter = (s) => true;
        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            termFilter = (s) => words.All(w => s?.Contains(w, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .OrderBy(e => Convert.ToInt32(e))
            .Select(e =>
                new
                {
                    description = EnumUtils.GetEnumDescription((Enum)(object)e),
                    vo = new EnumNomVO<TEnum>(e)
                })
            .Where(e => termFilter(e.description))
            .Select(e => e.vo)
            .ToList();
    }
}
