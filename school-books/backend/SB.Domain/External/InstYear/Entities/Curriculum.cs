namespace SB.Domain;

public class Curriculum
{
    // EF constructor
    private Curriculum()
    {
    }

    // only used properties should be mapped

    public int CurriculumId { get; private set; }

    public int SchoolYear { get; private set; }

    public int InstitutionId { get; private set; }

    public int? CurriculumGroupNum { get; private set; }

    public int? SubjectId { get; private set; }

    public int? SubjectTypeId { get; private set; }

    public int? CurriculumPartID { get; private set; }

    public int? IsIndividualLesson { get; private set; }

    public bool? IsIndividualCurriculum { get; private set; }

    public int? TotalTermHours { get; private set; }

    public int? ParentCurriculumID { get; private set; }

    public bool IsValid { get; private set; }
}
