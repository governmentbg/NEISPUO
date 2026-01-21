namespace SB.Domain;

public static class FinalGradesModelSample
{
    public static readonly FinalGradesModel Sample =
        new(
            new FinalGradesModelCurriculumInfo[]
            {
                new FinalGradesModelCurriculumInfo(1, "Български език и литература", "ООП"),
                new FinalGradesModelCurriculumInfo(2, "Български език и литература", "ЗИП"),
                new FinalGradesModelCurriculumInfo(3, "Математика", "ООП"),
                new FinalGradesModelCurriculumInfo(4, "Изобразително изкуство", "ООП"),
                new FinalGradesModelCurriculumInfo(5, "Чужд език - Английски език", "ООП"),
                new FinalGradesModelCurriculumInfo(6, "Чужд език - Английски език", "РП/УП-А"),
                new FinalGradesModelCurriculumInfo(7, "Музика", "ООП"),
                new FinalGradesModelCurriculumInfo(8, "Физическо възпитание и спорт", "ООП"),
                new FinalGradesModelCurriculumInfo(9, "Технологии и предприемачество", "ООП"),
                new FinalGradesModelCurriculumInfo(10, "Околен свят", "ООП")
            },
            new FinalGradesModelStudentInfo[]
            {
                new FinalGradesModelStudentInfo(1, 1, "Валентина Найденова Кръстева", false),
                new FinalGradesModelStudentInfo(2, 2, "Венцислав Петров Геров", false),
                new FinalGradesModelStudentInfo(3, 3, "Георги Молов Кръстев", false),
                new FinalGradesModelStudentInfo(4, 4, "Гергана Найденова Кръстева", false),
                new FinalGradesModelStudentInfo(5, 5, "Гергана Жекова Минева", false),
                new FinalGradesModelStudentInfo(6, 6, "Глория Бончева Вътова", false),
                new FinalGradesModelStudentInfo(7, 7, "Даниел Петров Монов", false),
                new FinalGradesModelStudentInfo(8, 8, "Желязко Ганчев Нецов", false),
                new FinalGradesModelStudentInfo(9, 9, "Здравко Невенов Иванов", false),
                new FinalGradesModelStudentInfo(10, 10, "Златка Найденова Кръстева", false)
            },
            new FinalGradesModelItem[]
            {
                new FinalGradesModelItem(
                    1,
                    new FinalGradesModelItemByCurriculum[]
                    {
                        new FinalGradesModelItemByCurriculum(
                            1,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            2,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            3,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "6")),
                        new FinalGradesModelItemByCurriculum(
                            4,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            5,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            6,
                            new FinalGradesModelItemByType(
                                "6",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            7,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "6")),
                        new FinalGradesModelItemByCurriculum(
                            8,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            9,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            10,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                    }),
                new FinalGradesModelItem(
                    3,
                    new FinalGradesModelItemByCurriculum[]
                    {
                        new FinalGradesModelItemByCurriculum(
                            1,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            2,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            3,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            4,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            5,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            6,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            7,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            8,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            9,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                null)),
                    }),
                new FinalGradesModelItem(
                    4,
                    new FinalGradesModelItemByCurriculum[]
                    {
                        new FinalGradesModelItemByCurriculum(
                            1,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "6")),
                        new FinalGradesModelItemByCurriculum(
                            2,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            3,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            4,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            5,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            6,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "6")),
                        new FinalGradesModelItemByCurriculum(
                            7,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            8,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "6")),
                        new FinalGradesModelItemByCurriculum(
                            9,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            10,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                    }),
                new FinalGradesModelItem(
                    5,
                    new FinalGradesModelItemByCurriculum[]
                    {
                        new FinalGradesModelItemByCurriculum(
                            1,
                            new FinalGradesModelItemByType(
                                "5",
                                null,
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            2,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            3,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            4,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            5,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            6,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            7,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            8,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            9,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            10,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                    }),
                new FinalGradesModelItem(
                    6,
                    new FinalGradesModelItemByCurriculum[]
                    {
                        new FinalGradesModelItemByCurriculum(
                            1,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            2,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            3,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            4,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            5,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            6,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            7,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            8,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            9,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            10,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                    }),
                new FinalGradesModelItem(
                    7,
                    new FinalGradesModelItemByCurriculum[]
                    {
                        new FinalGradesModelItemByCurriculum(
                            1,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            2,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            3,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            4,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            5,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            6,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            7,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            8,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "6")),
                        new FinalGradesModelItemByCurriculum(
                            9,
                            new FinalGradesModelItemByType(
                                "5",
                                null,
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            10,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4"))
                    }),
                new FinalGradesModelItem(
                    8,
                    new FinalGradesModelItemByCurriculum[]
                    {
                        new FinalGradesModelItemByCurriculum(
                            1,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            2,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            3,
                            new FinalGradesModelItemByType(
                                "5",
                                null,
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            4,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            5,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            6,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            7,
                            new FinalGradesModelItemByType(
                                "5",
                                null,
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            8,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            9,
                            new FinalGradesModelItemByType(
                                "6",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            10,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4"))
                    }),
                new FinalGradesModelItem(
                    9,
                    new FinalGradesModelItemByCurriculum[]
                    {
                        new FinalGradesModelItemByCurriculum(
                            1,
                            new FinalGradesModelItemByType(
                                null,
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            2,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            3,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            4,
                            new FinalGradesModelItemByType(
                                "6",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            5,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            6,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            7,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            8,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            9,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            10,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4"))
                    }),
                new FinalGradesModelItem(
                    10,
                    new FinalGradesModelItemByCurriculum[]
                    {
                        new FinalGradesModelItemByCurriculum(
                            1,
                            new FinalGradesModelItemByType(
                                "5",
                                "6",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            2,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            3,
                            new FinalGradesModelItemByType(
                                "5",
                                "5",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            4,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "3")),
                        new FinalGradesModelItemByCurriculum(
                            5,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            6,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            7,
                            new FinalGradesModelItemByType(
                                "5",
                                "2",
                                null)),
                        new FinalGradesModelItemByCurriculum(
                            8,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            9,
                            new FinalGradesModelItemByType(
                                "5",
                                "4",
                                "4")),
                        new FinalGradesModelItemByCurriculum(
                            10,
                            new FinalGradesModelItemByType(
                                "5",
                                null,
                                "4")),
                    }
                )
            }
        );
}
