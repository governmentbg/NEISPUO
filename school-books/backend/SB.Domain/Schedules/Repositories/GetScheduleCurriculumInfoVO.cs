namespace SB.Domain;
public record GetScheduleCurriculumInfoVO(
    int CurriculumId,
    string CurriculumName,
    bool IsIndividualCurriculum);
