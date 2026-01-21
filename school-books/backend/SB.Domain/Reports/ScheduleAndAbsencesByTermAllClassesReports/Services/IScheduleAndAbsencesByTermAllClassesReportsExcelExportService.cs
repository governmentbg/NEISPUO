namespace SB.Domain;

using System.IO;
using static SB.Domain.IScheduleAndAbsencesByTermReportsQueryRepository;

public interface IScheduleAndAbsencesByTermAllClassesReportsExcelExportService
{
    void ExportAsync(
        string term,
        string classBookName,
        bool isDPLR,
        GetWeeksForAddVO[] weeks,
        Stream outputStream);
}
