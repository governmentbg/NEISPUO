namespace SB.Domain;

using System;

public static class StudentIndividualWorksModelSample
{
    public static readonly StudentIndividualWorksModel Sample =
        new(
            new StudentIndividualWorksModelIndividualWork[]
            {
                new StudentIndividualWorksModelIndividualWork(new DateTime(2022, 10, 29), "Запознаване с преподавателите и учебния процес."),
                new StudentIndividualWorksModelIndividualWork(new DateTime(2022, 9, 17), "Запознаване с преподавателите и учебния процес"),
                new StudentIndividualWorksModelIndividualWork(new DateTime(2022, 11, 8), "Извършена е дискусия с ученика относно несъгласито с част от учебната програма")
            }
        );
}
