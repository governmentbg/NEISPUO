namespace SB.Domain;
using System;

public static class StudentNotesModelSample
{
    public static readonly StudentNotesModel Sample =
        new(
            new StudentNotesModelNote[]
            {
                new StudentNotesModelNote(new DateTime(2023, 3, 15), "Глория Бончева Вътова", "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."),
                new StudentNotesModelNote(new DateTime(2023, 3, 16), "Глория Бончева Вътова", "Запознаване с преподавателите и учебния процес"),
                new StudentNotesModelNote(new DateTime(2023, 3, 17), "Желязко Ганчев Нецов", "Извършена е дискусия с ученика относно несъгласито с част от учебната програма"),
                new StudentNotesModelNote(new DateTime(2023, 3, 18), "Златка Найденова Кръстева", "Запознаване с преподавателите и учебния процес")
            }
        );
}
