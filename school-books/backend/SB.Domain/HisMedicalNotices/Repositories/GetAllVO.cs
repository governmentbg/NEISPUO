namespace SB.Domain;

public partial interface IHisMedicalNoticesQueryRepository
{
    public record GetAllVO(
        int HisMedicalNoticeId,
        string NrnMedicalNotice,
        string NrnExamination,
        string Identifier,
        string SchoolYears);
}
