namespace SB.Domain;

/// <summary>{{StudentGradelessCurriculumsDO.Summary}}</summary>
public class StudentGradelessCurriculumsDO
{
    /// <summary>{{StudentGradelessCurriculumsDO.PersonId}}</summary>
    public int? PersonId { get; init; }

    /// <summary>{{StudentGradelessCurriculumsDO.GradelessCurriculums}}</summary>
    public StudentGradelessCurriculumDO[]? GradelessCurriculums { get; init; }
}

/// <summary>{{StudentGradelessCurriculumDO.Summary}}</summary>
public class StudentGradelessCurriculumDO
{
    /// <summary>{{StudentGradelessCurriculumDO.CurriculumId}}</summary>
    public int CurriculumId { get; init; }

    /// <summary>{{StudentGradelessCurriculumDO.WithoutFirstTermGrade}}</summary>
    public bool WithoutFirstTermGrade { get; init; }

    /// <summary>{{StudentGradelessCurriculumDO.WithoutSecondTermGrade}}</summary>
    public bool WithoutSecondTermGrade { get; init; }

    /// <summary>{{StudentGradelessCurriculumDO.WithoutFinalGrade}}</summary>
    public bool WithoutFinalGrade { get; init; }
}
