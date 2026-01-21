namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using SB.Common;

public partial interface ISchedulesQueryRepository
{
    Task<GetClassBookScheduleForWeekVO> GetClassBookScheduleForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        int? personId,
        CancellationToken ct);

    Task<GetClassBookScheduleTableForWeekVO> GetClassBookScheduleTableForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        bool showIndividualCurriculum,
        int? personId,
        bool showOnlyStudentCurriculums = false,
        CancellationToken ct = default);

    Task<GetSchoolYearSettings> GetSchoolYearSettingsAsync(int schoolYear, int classBookId, CancellationToken ct);

    Task<TableResultVO<GetAllByClassBookVO>> GetAllByClassBookAsync(
        int schoolYear,
        int classBookId,
        bool isIndividualSchedule,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(int schoolYear, int instId, int classBookId, int scheduleId, CancellationToken ct);

    Task<GetUsedDatesWeeksVO> GetUsedDatesWeeksAsync(int schoolYear, int classBookId, bool isIndividualSchedule, int? personId, int? exceptScheduleId, CancellationToken ct);

    Task<DateTime[]> GetUsedDatesAsync(int schoolYear, int classBookId, bool isIndividualSchedule, int? personId, int? exceptScheduleId, CancellationToken ct);

    Task<DateTime[]> GetOffDatesAsync(int schoolYear, int classBookId, CancellationToken ct);

    Task<GetOffDatesPgVO[]> GetOffDatesPgAsync(int schoolYear, int classBookId, CancellationToken ct);

    Task<GetShiftHoursForValidationVO[]> GetShiftHoursForValidationAsync(
        int schoolYear,
        int instId,
        int shiftId,
        CancellationToken ct);

    Task<GetScheduleUsedHoursVO[]> GetScheduleUsedHoursAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleId,
        CancellationToken ct);

    Task<GetScheduleUsedHoursTableVO[]> GetScheduleUsedHoursTableAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleId,
        int day,
        CancellationToken ct);

    Task<GetTeacherScheduleTableForWeekVO> GetTeacherScheduleTableForWeekAsync(
        int schoolYear,
        int instId,
        int teacherPersonId,
        int year,
        int weekNumber,
        CancellationToken ct);

    Task<GetScheduleCurriculumInfoVO[]> GetScheduleCurriculumsInfoAsync(
        int schoolYear,
        int instId,
        int[] curriculumIds,
        CancellationToken ct);
}
