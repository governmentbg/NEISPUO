namespace SB.Domain;

using System;

public static class StudentGradesModelSample
{
    public static readonly StudentGradesModel TermOneSample =
        new(
            Term: SchoolTerm.TermOne,
            new StudentGradesModelGrade[]
            {
                new StudentGradesModelGrade(new DateTime(2023, 3, 15), "Български език и литература / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 3, 19), "Околен свят / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 3, 28), "Български език и литература / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 2), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 3), "Математика / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 4), "Музика / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 5), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 6), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 6), "Физическо възпитание и спорт / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 6), "Изобразително изкуство / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 7), "Чужд език - Английски език / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 8), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 12), "Български език и литература / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 12), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 14), "Математика / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 3, 14), "Изобразително изкуство / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 3, 14), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 16), "Чужд език - Английски език / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 22), "Музика / ООП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 22), "Физическо възпитание и спорт / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 23), "Математика / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 24), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 24), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 26), "Чужд език - Английски език / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 30), "Математика / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 30), "Български език и литература / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 1), "Физическо възпитание и спорт / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 2), "Математика / ООП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 2), "Околен свят / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 15), "Музика / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 19), "Математика / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 28), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 2), "Изобразително изкуство / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 2), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 3), "Чужд език - Английски език / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 3), "Физическо възпитание и спорт / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 3), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 3), "Околен свят / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 4), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 4), "Изобразително изкуство / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 5), "Чужд език - Английски език / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 5), "Български език и литература / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 5), "Математика / ЗИП", "Отличен (6)")
            }
        );
    public static readonly StudentGradesModel TermTwoSample =
        new(
            Term: SchoolTerm.TermTwo,
            new StudentGradesModelGrade[]
            {
                new StudentGradesModelGrade(new DateTime(2023, 3, 15), "Български език и литература / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 3, 19), "Околен свят / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 3, 28), "Български език и литература / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 2), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 3), "Математика / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 4), "Музика / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 5), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 6), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 6), "Физическо възпитание и спорт / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 6), "Изобразително изкуство / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 7), "Чужд език - Английски език / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 8), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 12), "Български език и литература / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 12), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 14), "Математика / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 3, 14), "Изобразително изкуство / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 3, 14), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 16), "Чужд език - Английски език / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 22), "Музика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 22), "Физическо възпитание и спорт / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 23), "Математика / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 24), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 24), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 26), "Чужд език - Английски език / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 30), "Математика / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 4, 30), "Български език и литература / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 1), "Физическо възпитание и спорт / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 2), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 2), "Околен свят / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 15), "Музика / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 19), "Математика / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 28), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 2), "Изобразително изкуство / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 2), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 3), "Чужд език - Английски език / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 3), "Физическо възпитание и спорт / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 3), "Математика / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 3), "Околен свят / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 4), "Математика / ЗИП", "Отличен (6)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 4), "Изобразително изкуство / ООП", "Добър (4)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 5), "Чужд език - Английски език / ООП", "Мн. Добър (5)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 5), "Български език и литература / ЗИП", "Среден (3)"),
                new StudentGradesModelGrade(new DateTime(2023, 5, 5), "Математика / ЗИП", "Отличен (6)")
            }
        );
}
