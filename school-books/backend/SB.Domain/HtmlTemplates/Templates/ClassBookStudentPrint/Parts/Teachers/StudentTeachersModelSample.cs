namespace SB.Domain;

public static class StudentTeachersModelSample
{
    public static readonly StudentTeachersModel Sample =
        new(
            new StudentTeachersModelSubject[]
            {
                new StudentTeachersModelSubject("Родина", "ДП/ДрУП", "Десислава Дянкова"),
                new StudentTeachersModelSubject("Трудово", "ДП/ДрУП", "Таня Енева"),
                new StudentTeachersModelSubject("Татковина", "ДП/ДрУП", "Дебора Кирилова"),
                new StudentTeachersModelSubject("Работа с ученици и родители", "ОПЛР/ДопКонс", "Лилия Цанкова"),
                new StudentTeachersModelSubject("Работа с ученици и родители", "ОПЛР/ЛР", "Иделина Димитрова")
            }
        );
}
