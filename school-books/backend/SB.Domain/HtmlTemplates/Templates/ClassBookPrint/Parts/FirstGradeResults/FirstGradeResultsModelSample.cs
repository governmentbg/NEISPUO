namespace SB.Domain;
public static class FirstGradeResultsModelSample
{
    public static readonly FirstGradeResultsModel Sample =
        new(
            new FirstGradeResultsModelFirstGradeResult[]
            {
                new FirstGradeResultsModelFirstGradeResult(1, "Валентина Найденова Кръстева", false, QualitativeGrade.Excellent, null),
                new FirstGradeResultsModelFirstGradeResult(2, "Венцислав Петров Геров", false, QualitativeGrade.VeryGood, null),
                new FirstGradeResultsModelFirstGradeResult(3, "Георги Молов Кръстев", false, QualitativeGrade.Good, null),
                new FirstGradeResultsModelFirstGradeResult(4, "Гергана Найденова Кръстева", false, QualitativeGrade.Good, null),
                new FirstGradeResultsModelFirstGradeResult(5, "Гергана Жекова Минева", false, QualitativeGrade.Excellent, null),
                new FirstGradeResultsModelFirstGradeResult(6, "Глория Бончева Вътова", false, QualitativeGrade.VeryGood, null),
                new FirstGradeResultsModelFirstGradeResult(7, "Даниел Петров Монов", false, QualitativeGrade.Excellent, null),
                new FirstGradeResultsModelFirstGradeResult(8, "Желязко Ганчев Нецов", false, QualitativeGrade.Fair, null),
                new FirstGradeResultsModelFirstGradeResult(9, "Здравко Невенов Иванов", false, QualitativeGrade.VeryGood, null),
                new FirstGradeResultsModelFirstGradeResult(10, "Златка Найденова Кръстева", false, QualitativeGrade.Excellent, null),
                new FirstGradeResultsModelFirstGradeResult(11, "Михаела Томова Бинева", false, QualitativeGrade.Excellent, null)
            }
        );
}
