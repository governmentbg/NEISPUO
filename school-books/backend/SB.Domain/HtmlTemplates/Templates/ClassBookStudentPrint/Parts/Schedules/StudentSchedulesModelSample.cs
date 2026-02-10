namespace SB.Domain;

using System;

public static class StudentSchedulesModelSample
{
    public static readonly StudentSchedulesModel Sample =
        new(
            new StudentSchedulesModelSchedule[]
            {
                new StudentSchedulesModelSchedule(
                    new DateTime(2021, 09, 15),
                    new DateTime(2022, 02, 01),
                    new StudentSchedulesModelScheduleLessons[]
                    {
                        new StudentSchedulesModelScheduleLessons(1, 1, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(1, 2, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(1, 3, "Математика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(1, 4, "Изобразително изкуство", "ООП"),
                        new StudentSchedulesModelScheduleLessons(1, 5, "Изобразително изкуство", "ООП"),
                        new StudentSchedulesModelScheduleLessons(1, 6, "Час на класа", "…"),
                        new StudentSchedulesModelScheduleLessons(2, 1, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(2, 2, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(2, 3, "Чужд език - Английски език", "ООП"),
                        new StudentSchedulesModelScheduleLessons(2, 4, "Чужд език - Английски език", "РП/УП-А"),
                        new StudentSchedulesModelScheduleLessons(2, 5, "Музика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(2, 6, "Татковина", "ДП/ДрУП"),
                        new StudentSchedulesModelScheduleLessons(3, 1, "Физическо възпитание и спорт", "ООП"),
                        new StudentSchedulesModelScheduleLessons(3, 2, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(3, 3, "Математика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(3, 4, "Технологии и предприемачество", "ООП"),
                        new StudentSchedulesModelScheduleLessons(3, 5, "Околен свят", "ООП"),
                        new StudentSchedulesModelScheduleLessons(3, 6, "Родина", "ДП/ДрУП"),
                        new StudentSchedulesModelScheduleLessons(4, 1, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(4, 2, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(4, 3, "Математика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(4, 4, "Музика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(4, 5, "Спортни дейности (лека атлетика)", "…"),
                        new StudentSchedulesModelScheduleLessons(5, 1, "Физическо възпитание и спорт", "ООП"),
                        new StudentSchedulesModelScheduleLessons(5, 2, "Български език и литература", "РП/УП-А"),
                        new StudentSchedulesModelScheduleLessons(5, 3, "Математика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(5, 4, "Чужд език - Английски език", "ООП"),
                        new StudentSchedulesModelScheduleLessons(5, 5, "Чужд език - Английски език", "РП/УП-А"),
                        new StudentSchedulesModelScheduleLessons(5, 6, "Трудово", "ДП/ДрУП"),
                        new StudentSchedulesModelScheduleLessons(5, 7, "Трудово", "ДП/ДрУП"),
                        new StudentSchedulesModelScheduleLessons(5, 8, "Трудово", "ДП/ДрУП"),
                        new StudentSchedulesModelScheduleLessons(5, 9, "Трудово", "ДП/ДрУП"),
                    }),
                new StudentSchedulesModelSchedule(
                    new DateTime(2021, 11, 15),
                    new DateTime(2021, 11, 28),
                    new StudentSchedulesModelScheduleLessons[]
                    {
                        new StudentSchedulesModelScheduleLessons(1, 1, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(1, 2, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(1, 3, "Математика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(1, 4, "Изобразително изкуство", "ООП"),
                        new StudentSchedulesModelScheduleLessons(1, 5, "Изобразително изкуство", "ООП"),
                        new StudentSchedulesModelScheduleLessons(1, 6, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(2, 1, "Татковина", "ДП/ДрУП"),
                        new StudentSchedulesModelScheduleLessons(2, 2, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(2, 3, "Чужд език - Английски език", "ООП"),
                        new StudentSchedulesModelScheduleLessons(2, 4, "Чужд език - Английски език", "РП/УП-А"),
                        new StudentSchedulesModelScheduleLessons(2, 5, "Музика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(2, 6, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(3, 1, "Физическо възпитание и спорт", "ООП"),
                        new StudentSchedulesModelScheduleLessons(3, 2, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(3, 3, "Математика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(3, 4, "Технологии и предприемачество", "ООП"),
                        new StudentSchedulesModelScheduleLessons(3, 5, "Околен свят", "ООП"),
                        new StudentSchedulesModelScheduleLessons(3, 6, "Родина", "ДП/ДрУП"),
                        new StudentSchedulesModelScheduleLessons(4, 1, "Час на класа", "…"),
                        new StudentSchedulesModelScheduleLessons(4, 2, "Български език и литература", "ООП"),
                        new StudentSchedulesModelScheduleLessons(4, 3, "Математика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(4, 4, "Музика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(4, 5, "Спортни дейности (лека атлетика)", "…"),
                        new StudentSchedulesModelScheduleLessons(5, 1, "Физическо възпитание и спорт", "ООП"),
                        new StudentSchedulesModelScheduleLessons(5, 2, "Български език и литература", "РП/УП-А"),
                        new StudentSchedulesModelScheduleLessons(5, 3, "Математика", "ООП"),
                        new StudentSchedulesModelScheduleLessons(5, 4, "Чужд език - Английски език", "ООП"),
                        new StudentSchedulesModelScheduleLessons(5, 5, "Чужд език - Английски език", "РП/УП-А"),
                        new StudentSchedulesModelScheduleLessons(5, 6, "Трудово", "ДП/ДрУП"),
                    }),
                
            }
        );
}
