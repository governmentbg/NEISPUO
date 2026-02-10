namespace SB.Domain;
using System;

public static class StudentSupportsModelSample
{
    public static readonly StudentSupportsModel Sample =
        new(
            new StudentSupportsModelSupport[]
            {
                new StudentSupportsModelSupport(
                    "Напредва по-бързо от останалите ученици",
                    "Напредва по-бързо от останалите ученици",
                    null,
                    new DateTime(2022, 5, 15),
                    new [] {"Десислава Дянкова", "Таня Енева" },
                    new StudentSupportsModelActivity[]
                    {
                        new StudentSupportsModelActivity(
                            "Кариерно ориентиране",
                            new DateTime(2022, 5, 15),
                            "Насоки за кариерно развитие",
                            null
                        )
                    }
                ),
                new StudentSupportsModelSupport(
                    "Затруднения в обучението",
                    "Изпитва затруднения в усвояването на материала",
                    "Да повиши концентрацията си",
                    new DateTime(2022, 6, 13),
                    new [] {"Десислава Дянкова", "Таня Енева" },
                    new StudentSupportsModelActivity[]
                    {
                        new StudentSupportsModelActivity(
                            "Допълнително обучение по учебен предмет",
                            new DateTime(2022, 6, 13),
                            "По-добро усвояване на материала",
                            null
                        ),
                        new StudentSupportsModelActivity(
                            "Консултации",
                            new DateTime(2022, 6, 14),
                            "Съвети за начин на учене",
                            "Наблюдава се прогрес"
                        )
                    }
                ),
                new StudentSupportsModelSupport(
                    "Затруднения в обучението",
                    null,
                    null,
                    new DateTime(2022, 6, 15),
                    new [] {"Десислава Дянкова", "Таня Енева" },
                    new StudentSupportsModelActivity[]
                    {
                        new StudentSupportsModelActivity(
                            "Допълнително обучение по учебен предмет",
                            new DateTime(2022, 6, 15),
                            "По-добро усвояване на материала",
                            null
                        ),
                        new StudentSupportsModelActivity(
                            "Допълнително обучение по учебен предмет",
                            new DateTime(2022, 6, 16),
                            null,
                            null
                        ),
                        new StudentSupportsModelActivity(
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
