namespace SB.Domain;

public static class StudentGradeResultSessionsModelSample
{
    public static readonly StudentGradeResultSessionsModel Sample =
        new(
            new StudentGradeResultSessionsModelSession[]
                {
                    new StudentGradeResultSessionsModelSession(
                        "Математика ООП",
                        "Добър(4)",
                        null,
                        null
                    ),
                    new StudentGradeResultSessionsModelSession(
                        "Български език и литература / ООП",
                        "Неявил се",
                        "Добър(4)",
                        null
                    ),
                    new StudentGradeResultSessionsModelSession(
                        "Музика ООП",
                        "Слаб(2)",
                        "Неявил се",
                        null
                    ),
                    new StudentGradeResultSessionsModelSession(
                        "Чужд език - Английски език / РП/УП-А",
                        "Слаб(2)",
                        "Неявил се",
                        null
                    )
                },
            null
        );
}
