DECLARE @SchoolYear INT = 2024;
DECLARE @MedicalAbsenceReasonId INT = 1;
DECLARE @SchoolBooksSysUserId INT = 3;

-- Update Attendances with matching HisMedicalNotice
WITH UnexcusedAttendances AS (
    -- Step 1: Select all unexcused attendances
    SELECT a.AttendanceId, a.PersonId, a.[Date]
    FROM [school_books].[Attendance] a
    WHERE a.SchoolYear = @SchoolYear
        AND a.HisMedicalNoticeId IS NULL
        AND a.[Type] = 2
        AND NOT EXISTS (
            SELECT 1
            FROM [school_books].[ClassBook] cb
            INNER JOIN [school_books].[ClassBookExtProvider] cbep
                ON cb.SchoolYear = cbep.SchoolYear
                AND cb.InstId = cbep.InstId
            WHERE cb.SchoolYear = a.SchoolYear
                AND cb.ClassBookId = a.ClassBookId
                AND cbep.ExtSystemId IS NOT NULL
        )
),
MatchingMedicalNotices AS (
    -- Step 2: Match attendances to medical notices
    SELECT ua.AttendanceId, hmn.HisMedicalNoticeId, hmn.NrnMedicalNotice, hmn.Pmi, 
           hmn.AuthoredOn, hmn.FromDate, hmn.ToDate
    FROM UnexcusedAttendances ua
    INNER JOIN [school_books].[PersonMedicalNotice] pmn 
        ON pmn.SchoolYear = @SchoolYear AND ua.PersonId = pmn.PersonId
    INNER JOIN [school_books].[HisMedicalNotice] hmn 
        ON pmn.HisMedicalNoticeId = hmn.HisMedicalNoticeId
        AND hmn.FromDate <= ua.[Date]
        AND hmn.ToDate >= ua.[Date]
)
UPDATE a
SET 
    a.HisMedicalNoticeId = mmn.HisMedicalNoticeId,
    a.[Type] = 3,
    a.ExcusedReasonId = @MedicalAbsenceReasonId,
    a.ExcusedReasonComment = CASE 
        WHEN mmn.FromDate = mmn.ToDate THEN 
            N'Автоматично уважено с електронна медицинска бележка ' 
            + mmn.NrnMedicalNotice + '/' + FORMAT(mmn.AuthoredOn, 'dd.MM.yyyy') 
            + N' за дата ' + FORMAT(mmn.FromDate, 'dd.MM.yyyy') 
            + N'. УИН на лекаря - ' + mmn.Pmi + N'.'
        ELSE
            N'Автоматично уважено с електронна медицинска бележка ' 
            + mmn.NrnMedicalNotice + '/' + FORMAT(mmn.AuthoredOn, 'dd.MM.yyyy') 
            + N' за периода ' + FORMAT(mmn.FromDate, 'dd.MM.yyyy') 
            + N' - ' + FORMAT(mmn.ToDate, 'dd.MM.yyyy') 
            + N'. УИН на лекаря - ' + mmn.Pmi + N'.'
    END,
    a.ModifyDate = GETDATE(),
    a.ModifiedBySysUserId = @SchoolBooksSysUserId
FROM [school_books].[Attendance] a
INNER JOIN MatchingMedicalNotices mmn 
    ON a.AttendanceId = mmn.AttendanceId;
