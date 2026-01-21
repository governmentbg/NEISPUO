namespace SB.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using SB.Common;
using SB.Domain;

class GradeTypeNomsRepository : EnumNomsRepository<GradeType>, IGradeTypeNomsRepository
{
    public GradeTypeNomsRepository()
    {
    }

    public IList<EnumNomVO<GradeType>> GetNomsByTerm(bool showFinalGradeType, bool showTermGradeType, bool showGradeTypesWithoutScheduleLesson, string? term)
    {
        Func<string?, bool> termFilter = (s) => true;
        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            termFilter = (s) => words.All(w => s?.Contains(w, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        var result = Enum.GetValues(typeof(GradeType))
            .Cast<GradeType>()
            .OrderBy(e => Convert.ToInt32(e))
            .Select(e =>
                new
                {
                    description = EnumUtils.GetEnumDescription((Enum)(object)e),
                    vo = new EnumNomVO<GradeType>(e)
                })
            .Where(e => termFilter(e.description))
            .Select(e => e.vo)
            .ToList();

        if (!showGradeTypesWithoutScheduleLesson)
        {
            return result
                .Where(x => Grade.GradeTypeRequiresScheduleLesson(x.Id))
                .ToList();
        }

        var filteredResult = result;

        if (!showFinalGradeType)
        {
            filteredResult = filteredResult
                .Where(x => x.Id != GradeType.Final)
                .ToList();
        }

        if (!showTermGradeType)
        {
            filteredResult = filteredResult
                .Where(x =>
                    x.Id != GradeType.Term &&
                    x.Id != GradeType.OtherClassTerm &&
                    x.Id != GradeType.OtherSchoolTerm)
                .ToList();
        }

        return filteredResult;
    }
}
