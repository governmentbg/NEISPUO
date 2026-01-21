namespace SB.Domain;
using System;

public static class SupportsModelSample
{
    public static readonly SupportsModel Sample =
        new(
            new SupportsModelSupport[]
            {
                new SupportsModelSupport(
                    new [] {"6. Глория Бончева Вътова" },
                    "Напредва по-бързо от останалите ученици",
                    "Напредва по-бързо от останалите ученици",
                    null,
                    new DateTime(2022, 5, 15),
                    new [] {"Десислава Дянкова", "Таня Енева" },
                    new SupportsModelActivity[]
                    {
                        new SupportsModelActivity(
                            "Кариерно ориентиране",
                            new DateTime(2022, 5, 15),
                            "Насоки за кариерно развитие",
                            null
                        )
                    }
                ),
                new SupportsModelSupport(
                    new [] { "3. Георги Молов Кръстев", "8. Желязко Ганчев Нецов" },
                    "Затруднения в обучението",
                    "Изпитва затруднения в усвояването на материала",
                    "Да повиши концентрацията си",
                    new DateTime(2022, 6, 13),
                    new [] {"Десислава Дянкова", "Таня Енева" },
                    new SupportsModelActivity[]
                    {
                        new SupportsModelActivity(
                            "Допълнително обучение по учебен предмет",
                            new DateTime(2022, 6, 13),
                            "По-добро усвояване на материала",
                            null
                        ),
                        new SupportsModelActivity(
                            "Консултации",
                            new DateTime(2022, 6, 14),
                            "Съвети за начин на учене",
                            "Наблюдава се прогрес"
                        )
                    }
                ),
                new SupportsModelSupport(
                    new [] {"10. Златка Найденова Кръстева" },
                    "Затруднения в обучението",
                    null,
                    null,
                    new DateTime(2022, 6, 15),
                    new [] {"Десислава Дянкова", "Таня Енева" },
                    new SupportsModelActivity[]
                    {
                        new SupportsModelActivity(
                            "Допълнително обучение по учебен предмет",
                            new DateTime(2022, 6, 15),
                            "По-добро усвояване на материала",
                            null
                        ),
                        new SupportsModelActivity(
                            "Допълнително обучение по учебен предмет",
                            new DateTime(2022, 6, 16),
                            null,
                            null
                        ),
                        new SupportsModelActivity(
                            "Консултации",
                            new DateTime(2022, 6, 17),
                            "Съвети за начин на учене",
                            null
                        )
                    }
                )
            }
        );
}
