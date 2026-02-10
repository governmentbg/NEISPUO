namespace SB.Domain;
using System;

public static class StudentAbsencesByTermModelSample
{
    public static readonly StudentAbsencesByTermModel TermOneSample =
        new(SchoolTerm.TermOne, new DateTime(22, 9, 15), new DateTime(23, 2, 3), 5, 3.5M);

    public static readonly StudentAbsencesByTermModel TermTwoSample =
        new(SchoolTerm.TermOne, new DateTime(23, 2, 4), new DateTime(23, 6, 30), 2, 1M);
}
