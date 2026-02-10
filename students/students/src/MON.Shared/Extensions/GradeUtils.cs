namespace MON.Shared.Extensions
{
    public static class GradeUtils
    {
        public static string GetDecimalGradeText(decimal? gradeValue)
        {
            return gradeValue switch
            {
                var gv when gv < 3.0m => "Слаб",
                var gv when gv < 3.5m => "Среден",
                var gv when gv < 4.5m => "Добър",
                var gv when gv < 5.5m => "Много добър",
                var gv when gv >= 5.5m => "Отличен",
                _ => string.Empty
            };
        }

        public static string GetQualitativeGradeText(int? grade)
        {
            return grade switch
            {
                2 => "Незадоволителен",
                3 => "Среден",
                4 => "Добър",
                5 => "Много добър",
                6 => "Отличен",
                _ => string.Empty
            };
        }

        public static string GetSpecialNeedsGradeText(int? grade)
        {
            return grade switch
            {
                1 => "Среща затруднения",
                2 => "Справя се",
                3 => "Постига изискванията",
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
                var gv when gv < 5.5m => $"Много добър({gradeValue:0.00})",
                var gv when gv >= 5.5m => $"Отличен({gradeValue:0.00})",
                _ => string.Empty
            };
        }
    }
}
