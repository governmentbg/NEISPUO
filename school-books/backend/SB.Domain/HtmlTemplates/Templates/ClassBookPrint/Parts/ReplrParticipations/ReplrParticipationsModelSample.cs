namespace SB.Domain;
using System;

public static class ReplrParticipationsModelSample
{
    public static readonly ReplrParticipationsModel Sample =
        new(
            new ReplrParticipationsModelReplrParticipation[]
            {
                new ReplrParticipationsModelReplrParticipation(
                    "Оценка от екип за подкрепа на личностното развитие (ЕПЛР) в детската градина/училището",
                    new DateTime(2022, 9, 15),
                    "Запознаване с преподавателите и учебния процес",
                    "Средно училище \"Вела Благоева\"",
                    "Гергана Найденова Кръстева"),
                new ReplrParticipationsModelReplrParticipation(
                    "Оценка на деца и ученици със специални образователни потребности при невъзможност за формиране на ЕПЛР в детската градина/училището",
                    new DateTime(2022, 9, 15),
                    null,
                    null,
                    "Гергана Найденова Кръстева"),
                new ReplrParticipationsModelReplrParticipation(
                    "Методическа подкрепа",
                    new DateTime(2022, 9, 15),
                    null,
                    "Средно училище \"Вела Благоева\"",
                    "Гергана Найденова Кръстева"),
                new ReplrParticipationsModelReplrParticipation(
                    "Преценка за обучението на ученик, настанен в болница",
                    new DateTime(2022, 9, 15),
                    "Запознаване с преподавателите и учебния процес",
                    "Средно училище \"Вела Благоева\"",
                    "Гергана Найденова Кръстева, Златка Найденова Кръстева, Иван Иванов Иванов"),
            }
        );
}
