namespace SB.Domain;

public static class StudentCoverPageModelSample
{
    public static readonly StudentCoverPageModel SampleRegular =
        new(
            "Валентина Найденова Кръстева",
            2021,
            "СУ \"Вела Благоева\"",
            "град",
            "Велико Търново",
            "Велико Търново",
            null,
            "Велико Търново",
            "8 a",
            null,
            null
        );

    public static readonly StudentCoverPageModel SampleRegularWithSpeciality =
        new(
            "Валентина Найденова Кръстева",
            2021,
            "СУ \"Вела Благоева\"",
            "град",
            "Велико Търново",
            "Велико Търново",
            null,
            "Велико Търново",
            "8 a",
            null,
            "Топлотехника"
        );

    public static readonly StudentCoverPageModel SamplePG =
        new(
            "Валентина Найденова Кръстева",
            2021,
            "СУ \"Вела Благоева\"",
            "град",
            "Велико Търново",
            "Велико Търново",
            null,
            "Велико Търново",
            "четвърта ПГ 5 и 6 г",
            null,
            null
        );

    public static readonly StudentCoverPageModel SampleCDO =
        new(
            "Валентина Найденова Кръстева",
            2021,
            "СУ \"Вела Благоева\"",
            "град",
            "Велико Търново",
            "Велико Търново",
            null,
            "Велико Търново",
            "2 в-ЦДО",
            null,
            null
        );

    public static readonly StudentCoverPageModel SampleDPLR =
        new(
            "Валентина Найденова Кръстева",
            2021,
            "СУ \"Вела Благоева\"",
            "град",
            "Велико Търново",
            "Велико Търново",
            null,
            "Велико Търново",
            "сборна заекв.",
            "Логопедична работа",
            null
        );
}
