namespace SB.Domain;

/// <summary>{{StudentSpecialNeedCurriculumsDO.Summary}}</summary>
public class StudentSpecialNeedCurriculumsDO
{
    /// <summary>{{StudentSpecialNeedCurriculumsDO.PersonId}}</summary>
    public int? PersonId { get; init; }

    /// <summary>{{StudentSpecialNeedCurriculumsDO.StudentSpecialNeedCurriculumIds}}</summary>
    public int[]? StudentSpecialNeedCurriculumIds { get; init; }

    /// <summary>{{StudentSpecialNeedCurriculumsDO.HasSpecialNeedFirstGradeResult}}</summary>
    public bool? HasSpecialNeedFirstGradeResult { get; init; }
}
