namespace SB.Domain;
public static class GradeResultSessionsModelSample
{
    public static readonly GradeResultSessionsModel Sample =
        new(
            new GradeResultSessionsModelStudent[]
            {
                new GradeResultSessionsModelStudent(
                    6,
                    "Глория Бончева Вътова",
                    false,
                    new GradeResultSessionsModelSession[]
                    {
                        new GradeResultSessionsModelSession(
                            "Математика ООП",
                            "Добър(4)",
                            null,
                            null
                        )
                    },
                    "Завършва"
                ),
                new GradeResultSessionsModelStudent(
                    8,
                    "Желязко Ганчев Нецов",
                    false,
                    new GradeResultSessionsModelSession[]
                    {
                        new GradeResultSessionsModelSession(
                            "Математика ООП",
                            "Неявил се",
                            "Среден(3)",
                            null
                        ),
                        new GradeResultSessionsModelSession(
                            "Български език и литература ООП",
                            "Неявил се",
                            "Неявил се",
                            "Среден(3)"
                        )
                    },
                    "Завършва"
                ),
                new GradeResultSessionsModelStudent(
                    10,
                    "Златка Найденова Кръстева",
                    false,
                    new GradeResultSessionsModelSession[]
                    {
                        new GradeResultSessionsModelSession(
                            "Математика ООП",
                            "Добър(4)",
                            null,
                            null
                        ),
                        new GradeResultSessionsModelSession(
                            "Български език и литература / ООП",
                            "Неявил се",
                            "Добър(4)",
                            null
                        ),
                        new GradeResultSessionsModelSession(
                            "Музика ООП",
                            "Слаб(2)",
                            "Неявил се",
                            null
                        ),
                        new GradeResultSessionsModelSession(
                            "Чужд език - Английски език / РП/УП-А",
                            "Слаб(2)",
                            "Неявил се",
                            null
                        )
                    },
                    null
                )
            }
        );
}
