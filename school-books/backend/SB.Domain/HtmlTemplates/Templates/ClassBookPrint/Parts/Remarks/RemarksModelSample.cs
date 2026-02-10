namespace SB.Domain;
using System;

public static class RemarksModelSample
{
    public static readonly RemarksModel Sample =
        new(
            new RemarksModelStudent[]
            {
                new RemarksModelStudent(
                    6,
                    "Глория Бончева Вътова",
                    false,
                    new RemarksModelRemark[]
                    {
                        new RemarksModelRemark(
                            new DateTime(2022, 7, 5),
                            "Забележка",
                            "Български език и литература",
                            "ЗП",
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam," +
                            "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." +
                            "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                        )
                    }
                ),
                new RemarksModelStudent(
                    8,
                    "Желязко Ганчев Нецов",
                    false,
                    new RemarksModelRemark[]
                    {
                        new RemarksModelRemark(
                            new DateTime(2022, 5, 7),
                            "Забележка",
                            "Математика",
                            "ЗП",
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam"
                        ),
                        new RemarksModelRemark(
                            new DateTime(2022, 5, 7),
                            "Похвала",
                            "Български език и литература",
                            "ЗП",
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit"
                        )
                    }
                ),
                new RemarksModelStudent(
                    10,
                    "Златка Найденова Кръстева",
                    false,
                    new RemarksModelRemark[]
                    {
                        new RemarksModelRemark(
                            new DateTime(2022, 5, 7),
                            "Забележка",
                            "Английски език",
                            "ЗИП",
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam," +
                            "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur."
                        ),
                        new RemarksModelRemark(
                            new DateTime(2022, 5, 7),
                            "Похвала",
                            "Български език и литература",
                            "ЗП",
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam"
                        ),
                        new RemarksModelRemark(
                            new DateTime(2022, 5, 7),
                            "Забележка",
                            "Математика",
                            "ЗП",
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam"
                        ),
                        new RemarksModelRemark(
                            new DateTime(2022, 5, 7),
                            "Забележка",
                            "Български език и литература",
                            "ЗИП",
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam," +
                            "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur."
                        )
                    }
                )
            }
        );
}
