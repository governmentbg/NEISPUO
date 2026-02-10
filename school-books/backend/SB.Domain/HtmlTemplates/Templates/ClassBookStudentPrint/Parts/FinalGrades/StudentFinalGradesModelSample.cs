namespace SB.Domain;

public static class StudentFinalGradesModelSample
{
    public static readonly StudentFinalGradesModel Sample =
        new(
            new StudentFinalGradesModelItem[]
            {
                new StudentFinalGradesModelItem(
                    "Български език и литература / ООП",
                    "5",
                    "6",
                    "4"),
                new StudentFinalGradesModelItem(
                    "Немски език / ООП",
                    "5",
                    "6",
                    "4"),
                new StudentFinalGradesModelItem(
                    "Математика / ООП",
                    "5",
                    "6",
                    "4"),
                new StudentFinalGradesModelItem(
                    "Околен свят / ООП",
                    "5",
                    "6",
                    "4"),
                new StudentFinalGradesModelItem(
                    "Уча и се забавлявам / ООП",
                    null,
                    "6",
                    "4"),
                new StudentFinalGradesModelItem(
                    "Музика / ООП",
                    "5",
                    null,
                    "4"),
                new StudentFinalGradesModelItem(
                    "Изобразително изкуство / ООП",
                    "5",
                    "6",
                    "4"),
                new StudentFinalGradesModelItem(
                    "Технологии и предприемачество / ООП",
                    "5",
                    "6",
                    "4"),
                new StudentFinalGradesModelItem(
                    "Физическо възпитание и спорт / ООП",
                    "5",
                    null,
                    "4")

            }
        );
}
