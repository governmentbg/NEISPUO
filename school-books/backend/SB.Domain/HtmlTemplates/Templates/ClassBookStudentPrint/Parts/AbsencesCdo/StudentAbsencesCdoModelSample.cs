namespace SB.Domain;

public static class StudentAbsencesCdoModelSample
{
    public static readonly StudentAbsencesCdoModel Sample =
        new(
            new StudentAbsencesItemByTypes(0, 2),
            new StudentAbsencesItemByTypes(0, 0),
            new StudentAbsencesItemByTypes(5, 0),
            new StudentAbsencesItemByTypes(0, 0),
            new StudentAbsencesItemByTypes(0, 0),
            new StudentAbsencesItemByTypes(0, 4.5M),
            new StudentAbsencesItemByTypes(0, 0),
            new StudentAbsencesItemByTypes(2, 0),
            new StudentAbsencesItemByTypes(0, 2),
            new StudentAbsencesItemByTypes(3, 0),
            new StudentAbsencesItemByTypes(0, 0),
            new StudentAbsencesItemByTypes(0, 0)
        );
}
