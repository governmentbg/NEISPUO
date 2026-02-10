namespace SB.Domain;
using System;

public static class StudentExamsModelSample
{
    public static readonly StudentExamsModel ClassworkSample =
        new(
            Type: BookExamType.ClassExam,
            new StudentExamsModelExam[]
            {
                new StudentExamsModelExam("Български език и литература", "ЗП", new DateTime[] { new DateTime(2021, 10, 29)}, new DateTime[] { new DateTime(2022, 4, 20) }),
                new StudentExamsModelExam("Математика", "ЗП", new DateTime[] { new DateTime(2021, 10, 25)}, new DateTime[] { new DateTime(2022, 4, 18) }),
                new StudentExamsModelExam("Английски език", "ЗП", new DateTime[] { new DateTime(2021, 10, 17)}, new DateTime[] { new DateTime(2022, 4, 20) })
            }
        );

    public static readonly StudentExamsModel Sample =
        new(
            Type: BookExamType.ControlExam,
            new StudentExamsModelExam[]
            {
                new StudentExamsModelExam(
                    "Български език и литература",
                    "ЗП",
                    new DateTime[]
                    {
                        new DateTime(2021, 10, 15),
                        new DateTime(2021, 12, 10)
                    },
                    new DateTime[]
                    {
                        new DateTime(2022, 2, 20),
                        new DateTime(2022, 5, 22)
                    }),
                new StudentExamsModelExam(
                    "Математика",
                    "ЗП",
                    new DateTime[]
                    {
                        new DateTime(2021, 10, 25),
                        new DateTime(2021, 11, 30)
                    },
                    new DateTime[]
                    {
                        new DateTime(2022, 4, 18),
                        new DateTime(2022, 5, 2)
                    }),
                new StudentExamsModelExam(
                    "Английски език",
                    "ЗИП",
                    new DateTime[]
                    {
                        new DateTime(2021, 10, 17)
                    },
                    new DateTime[]
                    {
                        new DateTime(2022, 4, 20)
                    })
            }
        );
}
