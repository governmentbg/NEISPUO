namespace SB.Domain;
public static class GradeUtils
{
    public static string GetDecimalGradeText(decimal? gradeValue)
    {
        return gradeValue switch
        {
            var gv when gv < 3.0m => "Слаб(2)",
            var gv when gv < 3.5m => "Среден(3)",
            var gv when gv < 4.5m => "Добър(4)",
            var gv when gv < 5.5m => "Мн.Добър(5)",
            var gv when gv >= 5.5m => "Отличен(6)",
            _ => string.Empty
        };
    }

    public static string GetFullDecimalGradeText(decimal? gradeValue)
    {
        return gradeValue switch
        {
            var gv when gv < 3.0m => $"Слаб({gradeValue:0.00})",
            var gv when gv < 3.5m => $"Среден({gradeValue:0.00})",
            var gv when gv < 4.5m => $"Добър({gradeValue:0.00})",
            var gv when gv < 5.5m => $"Мн.Добър({gradeValue:0.00})",
            var gv when gv >= 5.5m => $"Отличен({gradeValue:0.00})",
            _ => string.Empty
        };
    }

    public static string GetDecimalGradeTextOnly (decimal? gradeValue)
    {
        return gradeValue switch
        {
            var gv when gv < 3.0m => "Слаб",
            var gv when gv < 3.5m => "Среден",
            var gv when gv < 4.5m => "Добър",
            var gv when gv < 5.5m => "Мн.Добър",
            var gv when gv >= 5.5m => "Отличен",
            _ => string.Empty
        };
    }
}
