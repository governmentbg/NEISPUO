CREATE TABLE [school_books].[temp_Absence_deleted_20231116] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [AbsenceId]             INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [PersonId]              INT              NOT NULL,
    [Date]                  DATE             NOT NULL,
    [Type]                  INT              NOT NULL,
    [Term]                  INT              NOT NULL,
    [ExcusedReasonId]       INT              NULL,
    [ExcusedReasonComment]  NVARCHAR(1000)   NULL,
    [ScheduleLessonId]      INT              NOT NULL,
    [TeacherAbsenceId]      INT              NULL,
    [HisMedicalNoticeId]    INT              NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    PRIMARY KEY ([SchoolYear], [AbsenceId]),
);

DECLARE @schoolYear INT = 2023;
DECLARE @classBookId INT = 491556;

-- should be IsPGCompatible=1
SELECT
	IsPGCompatible = IIF(
		HasLateAbsences = 0 AND
		HasDplrAbsences = 0 AND
		HasGrades = 0 AND
		HasRemarks = 0 AND
		HasExams = 0 AND
		HasSanctions = 0 AND
		HasSupports = 0 AND
		HasFirstGradeResults = 0 AND
		HasIndividualWorks = 0 AND
		HasGradeResults = 0 AND
		HasPerformances = 0 AND
		HasReplrParticipations = 0,
		1,
		0)
FROM (
	SELECT
		cb.SchoolYear,
		cb.InstId,
		cb.ClassBookId,
		cb.BasicClassId,
		cg.ClassTypeId,
		HasGrades = CASE WHEN EXISTS (SELECT 1 FROM school_books.Grade g WHERE g.SchoolYear = cb.SchoolYear AND g.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasAbsences = CASE WHEN EXISTS (SELECT 1 FROM school_books.Absence a WHERE a.SchoolYear = cb.SchoolYear AND a.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
        HasLateAbsences = CASE WHEN EXISTS (SELECT 1 FROM school_books.Absence a WHERE a.SchoolYear = cb.SchoolYear AND a.ClassBookId = cb.ClassBookId AND TYPE = 1) THEN 1 ELSE 0 END,
		HasDplrAbsences = CASE WHEN EXISTS (SELECT 1 FROM school_books.Absence a WHERE a.SchoolYear = cb.SchoolYear AND a.ClassBookId = cb.ClassBookId AND TYPE IN (4,5)) THEN 1 ELSE 0 END,
		HasAttendances = CASE WHEN EXISTS (SELECT 1 FROM school_books.Attendance a WHERE a.SchoolYear = cb.SchoolYear AND a.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasTopics = CASE WHEN EXISTS (SELECT 1 FROM school_books.Topic t WHERE t.SchoolYear = cb.SchoolYear AND t.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasRemarks = CASE WHEN EXISTS (SELECT 1 FROM school_books.Remark r WHERE r.SchoolYear = cb.SchoolYear AND r.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasSchedules = CASE WHEN EXISTS (SELECT 1 FROM school_books.Schedule sch WHERE sch.SchoolYear = cb.SchoolYear AND sch.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasParentMeetings = CASE WHEN EXISTS (SELECT 1 FROM school_books.ParentMeeting pm WHERE pm.SchoolYear = cb.SchoolYear AND pm.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasExams = CASE WHEN EXISTS (SELECT 1 FROM school_books.Exam e WHERE e.SchoolYear = cb.SchoolYear AND e.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasSanctions = CASE WHEN EXISTS (SELECT 1 FROM school_books.Sanction s WHERE s.SchoolYear = cb.SchoolYear AND s.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasSupports = CASE WHEN EXISTS (SELECT 1 FROM school_books.Support s WHERE s.SchoolYear = cb.SchoolYear AND s.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasNotes = CASE WHEN EXISTS (SELECT 1 FROM school_books.Note n WHERE n.SchoolYear = cb.SchoolYear AND n.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasFirstGradeResults = CASE WHEN EXISTS (SELECT 1 FROM school_books.FirstGradeResult fgr WHERE fgr.SchoolYear = cb.SchoolYear AND fgr.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasPgResults = CASE WHEN EXISTS (SELECT 1 FROM school_books.PgResult pr WHERE pr.SchoolYear = cb.SchoolYear AND pr.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasIndividualWorks = CASE WHEN EXISTS (SELECT 1 FROM school_books.IndividualWork iw WHERE iw.SchoolYear = cb.SchoolYear AND iw.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasGradeResults = CASE WHEN EXISTS (SELECT 1 FROM school_books.GradeResult gr WHERE gr.SchoolYear = cb.SchoolYear AND gr.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasPerformances = CASE WHEN EXISTS (SELECT 1 FROM school_books.Performance p WHERE p.SchoolYear = cb.SchoolYear AND p.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END,
		HasReplrParticipations = CASE WHEN EXISTS (SELECT 1 FROM school_books.ReplrParticipation rp WHERE rp.SchoolYear = cb.SchoolYear AND rp.ClassBookId = cb.ClassBookId) THEN 1 ELSE 0 END
	FROM school_books.ClassBook cb
	JOIN inst_year.ClassGroup cg ON cb.SchoolYear = cg.SchoolYear AND cb.ClassId = cg.ClassID
	WHERE cb.SchoolYear = @schoolYear AND cb.ClassBookId = @classBookId
) t

INSERT INTO school_books.Attendance(
	SchoolYear,
	AttendanceId,
	ClassBookId,
	PersonId,
	Date,
	Type,
	ExcusedReasonId,
	ExcusedReasonComment,
	CreateDate,
	CreatedBySysUserId,
	ModifyDate,
	ModifiedBySysUserId
)
SELECT
    SchoolYear = @schoolYear,
    AttendanceId = NEXT VALUE FOR [school_books].[AttendanceIdSequence],
    ClassBookId = @classBookId,
    PersonId = a1.PersonId,
    Date = a1.Date,
    Type = a1.Type,
    ExcusedReasonId = a1.ExcusedReasonId,
    ExcusedReasonComment = a1.ExcusedReasonComment,
    CreateDate = a1.CreateDate,
    CreatedBySysUserId = a1.CreatedBySysUserId,
    ModifyDate = a1.ModifyDate,
    ModifiedBySysUserId = a1.ModifiedBySysUserId
FROM (
    SELECT
        a.PersonId,
        a.Date,
        a.Type,
        a.ExcusedReasonId,
        a.ExcusedReasonComment,
        a.CreateDate,
        a.CreatedBySysUserId,
        a.ModifyDate,
        a.ModifiedBySysUserId,
        RowNumber = ROW_NUMBER() OVER (PARTITION BY a.PersonId, a.Date ORDER BY a.AbsenceId)
    FROM school_books.Absence a
    WHERE a.SchoolYear = @schoolYear
        AND a.ClassBookId = @classBookId
        AND a.Type IN (2, 3)
) a1
WHERE a1.RowNumber = 1;

DELETE a
OUTPUT
	DELETED.SchoolYear,
	DELETED.AbsenceId,
	DELETED.ClassBookId,
	DELETED.PersonId,
	DELETED.Date,
	DELETED.Type,
	DELETED.Term,
	DELETED.ExcusedReasonId,
	DELETED.ExcusedReasonComment,
	DELETED.ScheduleLessonId,
	DELETED.TeacherAbsenceId,
	DELETED.HisMedicalNoticeId,
	DELETED.CreateDate,
	DELETED.CreatedBySysUserId,
	DELETED.ModifyDate,
	DELETED.ModifiedBySysUserId
INTO school_books.temp_Absence_deleted_20231116 (
	SchoolYear,
	AbsenceId,
    ClassBookId,
	PersonId,
	Date,
	Type,
	Term,
	ExcusedReasonId,
	ExcusedReasonComment,
	ScheduleLessonId,
	TeacherAbsenceId,
	HisMedicalNoticeId,
    CreateDate,
	CreatedBySysUserId,
	ModifyDate,
	ModifiedBySysUserId
)
FROM school_books.Absence a
WHERE a.SchoolYear = @schoolYear
	AND a.ClassBookId = @classBookId
	AND a.Type IN (2, 3);

UPDATE school_books.ClassBook
SET BookType = 1
WHERE SchoolYear = @schoolYear
	AND ClassBookId = @classBookId

-- should return 0 rows
SELECT * FROM school_books.Absence a
WHERE a.SchoolYear = @schoolYear
	AND a.ClassBookId = @classBookId;
