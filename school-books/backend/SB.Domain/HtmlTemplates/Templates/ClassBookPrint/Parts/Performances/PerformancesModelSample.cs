namespace SB.Domain;
using System;

public static class PerformancesModelSample
{
    public static readonly PerformancesModel Sample =
        new(
            new PerformancesModelPerformance[]
            {
                new PerformancesModelPerformance(
                    "Организирана от ЦПЛР",
                    "Тест име 1",
                    "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                    new DateTime(2022, 5, 10),
                    new DateTime(2022, 9, 10),
                    "София",
                    null),
                new PerformancesModelPerformance(
                    "Организирана от ЦПЛР",
                    "Тест име 2",
                    "Запознаване с преподавателите и учебния процес",
                    new DateTime(2022, 8, 10),
                    new DateTime(2022, 9, 10),
                    "Пловдив",
                    "Награда 1, награда 2, награда 3"),
                new PerformancesModelPerformance(
                    "На общинско ниво",
                    "Тест име 3",
                    "Извършена е дискусия с ученика относно несъгласито с част от учебната програма",
                    new DateTime(2022, 2, 10),
                    new DateTime(2022, 5, 10),
                    "София",
                    "Награда 1, награда 2, награда 3, Награда 4, награда 5, награда 6"),
                new PerformancesModelPerformance(
                    "На регионално ниво",
                    "Тест име 4",
                    "Запознаване с преподавателите и учебния процес",
                    new DateTime(2022, 5, 10),
                    new DateTime(2022, 11, 10),
                    "София",
                    null),
            }
        );
}
