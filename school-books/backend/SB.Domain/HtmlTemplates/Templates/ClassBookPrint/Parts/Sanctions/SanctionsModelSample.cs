namespace SB.Domain;
using System;

public static class SanctionsModelSample
{
    public static readonly SanctionsModel Sample =
        new(
            new SanctionsModelStudent[]
            {
                new SanctionsModelStudent(
                    6,
                    "Глория Бончева Вътова",
                    false,
                    new SanctionsModelSanction[]
                    {
                        new SanctionsModelSanction(
                            "чл.199 ал.1 т.1 Забележка",
                            "431a",
                            new DateTime(2022, 7, 5),
                            "431a",
                            new DateTime(2022, 7, 15)
                        )
                    }
                ),
                new SanctionsModelStudent(
                    8,
                    "Желязко Ганчев Нецов",
                    false,
                    new SanctionsModelSanction[]
                    {
                        new SanctionsModelSanction(
                            "чл.199 ал.1 т.1 Забележка",
                            "431a",
                            new DateTime(2022, 5, 7),
                            "431a",
                            new DateTime(2022, 7, 15)
                        ),
                        new SanctionsModelSanction(
                            "чл.199 ал.1 т.2 Преместване в друга паралелка в същото училище",
                            "4a31a",
                            new DateTime(2022, 5, 7),
                            null,
                            null
                        )
                    }
                ),
                new SanctionsModelStudent(
                    10,
                    "Златка Найденова Кръстева",
                    false,
                    new SanctionsModelSanction[]
                    {
                        new SanctionsModelSanction(
                            "чл.199 ал.1 т.1 Забележка",
                            "43a1a",
                            new DateTime(2022, 5, 7),
                            "431a",
                            new DateTime(2022, 7, 15)
                        ),
                        new SanctionsModelSanction(
                            "чл.199 ал.1 т.1 Забележка",
                            "4a31a",
                            new DateTime(2022, 5, 7),
                            null,
                            null
                        ),
                        new SanctionsModelSanction(
                            "чл.199 ал.1 т.3 Предупреждение за преместване в друго училище",
                            "431a",
                            new DateTime(2022, 5, 7),
                            "431a",
                            new DateTime(2022, 7, 15)
                        ),
                        new SanctionsModelSanction(
                            "чл.199 ал.1 т.1 Забележка",
                            "431a",
                            new DateTime(2022, 5, 7),
                            null,
                            null
                        )
                    }
                )
            }
        );
}
