namespace SB.Data;

public partial interface IInstitutionCurriculumNomsRepository
{
    public record InstitutionCurriculumNomVO(InstitutionCurriculumNomVOCurriculum Id, string Name);

    public record InstitutionCurriculumNomVOCurriculum(int SubjectId, int SubjectTypeId);
}
