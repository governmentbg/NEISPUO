namespace SB.Domain;

using System;

public static class IndividualWorksModelSample
{
    public static readonly IndividualWorksModel Sample =
        new(
            new IndividualWorksModelIndividualWork[]
            {
                new IndividualWorksModelIndividualWork(4, "Гергана Найденова Кръстева", false, new DateTime(2022, 10, 29), "Запознаване с преподавателите и учебния процес."),
                new IndividualWorksModelIndividualWork(6, "Глория Бончева Вътова", false, new DateTime(2022, 9, 17), "Запознаване с преподавателите и учебния процес"),
                new IndividualWorksModelIndividualWork(8, "Желязко Ганчев Нецов", false, new DateTime(2022, 11, 8), "Извършена е дискусия с ученика относно несъгласито с част от учебната програма")
            }
        );
}
