namespace SB.Domain;
public static class NotesModelSample
{
    public static readonly NotesModel Sample =
        new(
            new NotesModelNote[]
            {
                new NotesModelNote("Гергана Найденова Кръстева, Глория Бончева Вътова", "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."),
                new NotesModelNote(null, "Запознаване с преподавателите и учебния процес"),
                new NotesModelNote("Желязко Ганчев Нецов", "Извършена е дискусия с ученика относно несъгласито с част от учебната програма"),
                new NotesModelNote("Златка Найденова Кръстева", "Запознаване с преподавателите и учебния процес")
            }
        );
}
