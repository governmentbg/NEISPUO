namespace SB.Domain;

public static class TeachersModelSample
{
    public static readonly TeachersModel Sample =
        new(
            new TeachersModelSubject[]
            {
                new TeachersModelSubject("Родина", "ДП/ДрУП", "Десислава Дянкова"),
                new TeachersModelSubject("Трудово", "ДП/ДрУП", "Таня Енева"),
                new TeachersModelSubject("Татковина", "ДП/ДрУП", "Дебора Кирилова"),
                new TeachersModelSubject("Работа с ученици и родители", "ОПЛР/ДопКонс", "Лилия Цанкова"),
                new TeachersModelSubject("Работа с ученици и родители", "ОПЛР/ЛР", "Иделина Димитрова")
            }
        );
}
