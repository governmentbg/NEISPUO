namespace SB.Domain;

public partial interface ISpbsBookRecordsQueryRepository
{
    public record GetAllVO(
        int SpbsBookRecordId,
        int SchoolYear,
        int RecordNumber,
        string StudentName,
        string PersonalId,
        string Gender);
}
