namespace SB.Data;

using System.Collections.Generic;
using SB.Domain;

public interface IGradeTypeNomsRepository : IEnumNomsRepository<GradeType>
{
    IList<EnumNomVO<GradeType>> GetNomsByTerm(bool showFinalGradeType, bool showTermGradeType, bool showGradeTypesWithoutScheduleLesson, string? term);
}
