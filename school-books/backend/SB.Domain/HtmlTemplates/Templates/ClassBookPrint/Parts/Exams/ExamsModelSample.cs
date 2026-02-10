namespace SB.Domain;
using System;

public static class ExamsModelSample
{
    public static readonly ExamsModel ClassworkSample =
        new(
            Type: BookExamType.ClassExam,
            new ExamsModelExam[]
            {
                new ExamsModelExam("Български език и литература", "ЗП", new DateTime[] { new DateTime(2021, 10, 29)}, new DateTime[] { new DateTime(2022, 4, 20) }),
                new ExamsModelExam("Математика", "ЗП", new DateTime[] { new DateTime(2021, 10, 25)}, new DateTime[] { new DateTime(2022, 4, 18) }),
                new ExamsModelExam("Английски език", "ЗП", new DateTime[] { new DateTime(2021, 10, 17)}, new DateTime[] { new DateTime(2022, 4, 20) })
            }
        );

    public static readonly ExamsModel Sample =
        new(
            Type: BookExamType.ControlExam,
            new ExamsModelExam[]
            {
                new ExamsModelExam(
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
                new ExamsModelExam(
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
                new ExamsModelExam(
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
