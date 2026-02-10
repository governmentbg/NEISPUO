PRINT 'Create Get_DBP_StudentSchedule stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_DBP_StudentSchedule]
    @schoolYear INT,
    @azureID VARCHAR(100),
    @fromDate DATE,
    @toDate DATE
AS
    DECLARE @personId INT;
    SET @personId = (SELECT PersonId FROM [core].[Person] WHERE AzureID = @azureID);

    CREATE TABLE #StudentClassBooks (
        [SchoolYear] INT NOT NULL,
        [InstId] INT NOT NULL,
        [ClassBookId] INT NOT NULL,
        [ClassId] INT NOT NULL
    );

    INSERT #StudentClassBooks
    SELECT
        [SchoolYear],
        [InstId],
        [ClassBookId],
        [ClassId]
    FROM
        [school_books].[vwStudentClassBooks]
    WHERE
        PersonId = @personId AND
        SchoolYear = @schoolYear;

    SELECT
        [SchoolYear] = cb.SchoolYear,
        [InstitutionId] = cb.InstId,
        [ClassId] = cb.ClassId,
        [ClassName] = cg.ClassName,
        [BasicClassId] = cg.BasicClassID,
        [ParentClassId] = cg.ParentClassId,
        [ScheduleLessonId] = sl.ScheduleLessonId,
        [ShiftId] = sch.ShiftId,
        [ShiftName] = sh.Name,
        [IsIndividualCurriculum] = sch.IsIndividualCurriculum,
        [Date] = sl.Date,
        [Day] = sl.Day,
        [HourNumber] = sl.HourNumber,
        [StartTime] = shh.StartTime,
        [EndTime] = shh.EndTime,
        [CurriculumId] = c.CurriculumId,
        [CurriculumGroupName] = c.CurriculumGroupNum,
        [SubjectId] = s.SubjectID,
        [SubjectName] = s.SubjectName,
        [SubjectNameShort] = s.SubjectNameShort,
        [SubjectTypeId] = st.SubjectTypeID,
        [SubjectTypeName] = st.Name,
        [TeacherAzureIds] =
            IIF(tah.TeacherAbsenceId IS NULL,
                STRING_AGG(p.AzureID, ', ') WITHIN GROUP(ORDER BY p.AzureID),
                repl.AzureID),
        [TeacherNames] =
            IIF(tah.TeacherAbsenceId IS NULL,
                STRING_AGG(CONCAT_WS(' ', p.FirstName, p.LastName), ', ') WITHIN GROUP(ORDER BY p.AzureID),
                NULLIF(CONCAT_WS(' ', repl.FirstName, repl.LastName), '')),
        [ReplTeacherIsNonSpecialist] = tah.ReplTeacherIsNonSpecialist,
        [IsEmptyHour] = IIF(tah.TeacherAbsenceId IS NOT NULL AND tah.ReplTeacherPersonId IS NULL, CAST(1 AS BIT), CAST(0 AS BIT)),
		[ProfCurriculumId] = pc.CurriculumID,
		[ProfSubjectId] = pc.SubjectID,
		[ProfSubjectName] = pcs.SubjectName,
		[ProfSubjectTypeId] = pc.SubjectTypeID
    FROM
        #StudentClassBooks cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Schedule] sch ON cb.SchoolYear = sch.SchoolYear AND cb.ClassBookId = sch.ClassBookId
        INNER JOIN [school_books].[Shift] sh ON sch.SchoolYear = sh.SchoolYear AND sch.ShiftId = sh.ShiftId
        INNER JOIN [school_books].[ScheduleLesson] sl ON sch.SchoolYear = sl.SchoolYear AND sch.ScheduleId = sl.ScheduleId
        INNER JOIN [school_books].[ShiftHour] shh ON sl.SchoolYear = shh.SchoolYear AND sch.ShiftId = shh.ShiftId AND sl.Day = shh.Day AND sl.HourNumber = shh.HourNumber
        INNER JOIN [inst_year].[Curriculum] c ON sl.CurriculumId = c.CurriculumID
		LEFT JOIN [inst_year].[Curriculum] pc ON c.ParentCurriculumID = pc.CurriculumID
        INNER JOIN [inst_year].[CurriculumStudent] cs ON cb.SchoolYear = cs.SchoolYear AND sl.CurriculumId = cs.CurriculumID AND cs.PersonID = @personId AND cs.Isvalid = 1
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
		LEFT JOIN [inst_nom].[Subject] pcs ON pc.SubjectID = pcs.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
        INNER JOIN [inst_year].[CurriculumTeacher] t ON c.CurriculumID = t.CurriculumID
        INNER JOIN [inst_basic].[StaffPosition] sp ON t.StaffPositionID = sp.StaffPositionID
        INNER JOIN [core].[Person] p ON sp.PersonID = p.PersonID
        LEFT JOIN [school_books].[TeacherAbsenceHour] tah ON sl.SchoolYear = tah.SchoolYear AND sl.ScheduleLessonId = tah.ScheduleLessonId
        LEFT JOIN [core].[Person] repl ON tah.ReplTeacherPersonId = repl.PersonID
    WHERE
        (sch.IsIndividualCurriculum = 0 OR sch.PersonId = @personId) AND
        sl.[Date] >= @fromDate AND
        sl.[Date] <= @toDate
    GROUP BY
        cb.SchoolYear,
        cb.InstId,
        cb.ClassId,
        cg.ClassName,
        cg.BasicClassID,
        cg.ParentClassId,
        sl.ScheduleLessonId,
        sch.ShiftId,
        sh.Name,
        sch.IsIndividualCurriculum,
        sl.Date,
        sl.Day,
        sl.HourNumber,
        shh.StartTime,
        shh.EndTime,
        c.CurriculumId,
		pc.CurriculumID,
		pc.SubjectID,
		pc.SubjectTypeID,
        c.CurriculumGroupNum,
        s.SubjectID,
        s.SubjectName,
		pcs.SubjectName,
        s.SubjectNameShort,
        st.SubjectTypeID,
        st.Name,
        tah.TeacherAbsenceId,
        tah.ReplTeacherPersonId,
        tah.ReplTeacherIsNonSpecialist,
        repl.AzureID,
        repl.FirstName,
        repl.LastName
    ORDER BY
        sl.Date,
        shh.StartTime;

    DROP TABLE #StudentClassBooks;
GO

PRINT 'Create Get_DBP_TeacherSchedule stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_DBP_TeacherSchedule]
    @schoolYear INT,
    @azureID VARCHAR(100),
    @fromDate DATE,
    @toDate DATE
AS
    DECLARE @personId INT;
    SET @personId = (SELECT PersonId FROM [core].[Person] WHERE AzureID = @azureID);

    CREATE TABLE #TeacherScheduleClassBooks (
        [SchoolYear] INT NOT NULL,
        [InstId] INT NOT NULL,
        [ClassBookId] INT NOT NULL,
        [ClassId] INT NOT NULL
    );

    INSERT #TeacherScheduleClassBooks
    SELECT DISTINCT
        [SchoolYear],
        [InstId],
        [ClassBookId],
        [ClassId]
    FROM
    (
        SELECT
            cb.SchoolYear,
            cb.InstId,
            cb.ClassBookId,
            cb.ClassId
        FROM
            [inst_basic].[StaffPosition] sp
            INNER JOIN [inst_year].[CurriculumTeacher] ct ON sp.StaffPositionId = ct.StaffPositionId
            INNER JOIN [school_books].[ScheduleLesson] sl ON ct.CurriculumId = sl.CurriculumId
            INNER JOIN [school_books].[Schedule] s ON sl.SchoolYear = s.SchoolYear AND sl.ScheduleId = s.ScheduleId
            INNER JOIN [school_books].[ClassBook] cb ON s.SchoolYear = s.SchoolYear AND cb.ClassBookId = s.ClassBookId
        WHERE
            sp.PersonId = @personId AND
            ct.SchoolYear = @schoolYear AND
            ct.IsValid = 1 AND
            cb.SchoolYear = @schoolYear

        UNION ALL

        SELECT
            cb.SchoolYear,
            cb.InstId,
            cb.ClassBookId,
            cb.ClassId
        FROM
            [school_books].[TeacherAbsenceHour] tah
            INNER JOIN [school_books].[ScheduleLesson] sl ON tah.SchoolYear = sl.SchoolYear AND tah.ScheduleLessonId = sl.ScheduleLessonId
            INNER JOIN [school_books].[Schedule] s ON sl.SchoolYear = s.SchoolYear AND sl.ScheduleId = s.ScheduleId
            INNER JOIN [school_books].[ClassBook] cb ON s.SchoolYear = cb.SchoolYear AND s.ClassBookId = cb.ClassBookId
        WHERE
            tah.SchoolYear = @schoolYear AND
            tah.ReplTeacherPersonId = @personId
    ) s;

    SELECT
        [SchoolYear] = cb.SchoolYear,
        [InstitutionId] = cb.InstId,
        [ClassId] = cb.ClassId,
        [ClassName] = cg.ClassName,
        [BasicClassId] = cg.BasicClassID,
        [ParentClassId] = cg.ParentClassId,
        [ScheduleLessonId] = sl.ScheduleLessonId,
        [ShiftId] = sch.ShiftId,
        [ShiftName] = sh.Name,
        [IsIndividualCurriculum] = sch.IsIndividualCurriculum,
        [IndividualCurriculumStudent] = NULLIF(CONCAT_WS(' ', ics.FirstName, ics.LastName), ''),
        [Date] = sl.Date,
        [Day] = sl.Day,
        [HourNumber] = sl.HourNumber,
        [StartTime] = shh.StartTime,
        [EndTime] = shh.EndTime,
        [CurriculumId] = c.CurriculumId,
        [CurriculumGroupName] = c.CurriculumGroupNum,
        [SubjectId] = s.SubjectID,
        [SubjectName] = s.SubjectName,
        [SubjectNameShort] = s.SubjectNameShort,
        [SubjectTypeId] = st.SubjectTypeID,
        [SubjectTypeName] = st.Name,
        [IsReplTeacher] = IIF(tah.TeacherAbsenceId IS NOT NULL AND tah.ReplTeacherPersonId = @personId, CAST(1 AS BIT), CAST(0 AS BIT)),
        [IsReplTeacherNonSpecialist] = tah.ReplTeacherIsNonSpecialist,
		[ProfCurriculumId] = pc.CurriculumID,
		[ProfSubjectId] = pc.SubjectID,
		[ProfSubjectTypeId] = pc.SubjectTypeID,
		[ProfSubjectName] = pcs.SubjectName
    FROM
        #TeacherScheduleClassBooks cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Schedule] sch ON cb.SchoolYear = sch.SchoolYear AND cb.ClassBookId = sch.ClassBookId
        INNER JOIN [school_books].[Shift] sh ON sch.SchoolYear = sh.SchoolYear AND sch.ShiftId = sh.ShiftId
        INNER JOIN [school_books].[ScheduleLesson] sl ON sch.SchoolYear = sl.SchoolYear AND sch.ScheduleId = sl.ScheduleId
        INNER JOIN [school_books].[ShiftHour] shh ON sl.SchoolYear = shh.SchoolYear AND sch.ShiftId = shh.ShiftId AND sl.Day = shh.Day AND sl.HourNumber = shh.HourNumber
        INNER JOIN [inst_year].[Curriculum] c ON sl.CurriculumId = c.CurriculumID
		LEFT JOIN [inst_year].[Curriculum] pc ON c.ParentCurriculumID = pc.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
		LEFT JOIN [inst_nom].[Subject] pcs ON pc.SubjectID = pcs.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
        LEFT JOIN [core].[Person] ics ON sch.PersonId = ics.PersonId
        LEFT JOIN [school_books].[TeacherAbsenceHour] tah ON sl.SchoolYear = tah.SchoolYear AND sl.ScheduleLessonId = tah.ScheduleLessonId
    WHERE
        sl.[Date] >= @fromDate AND
        sl.[Date] <= @toDate AND (
            (tah.TeacherAbsenceId IS NULL AND EXISTS (
                SELECT 1
                FROM [inst_year].[CurriculumTeacher] t
                    INNER JOIN [inst_basic].[StaffPosition] sp ON t.StaffPositionID = sp.StaffPositionID
                WHERE
                    t.CurriculumID = c.CurriculumID AND sp.PersonID = @personId
            )) OR
            (tah.TeacherAbsenceId IS NOT NULL AND tah.ReplTeacherPersonId = @personId)
        )
    ORDER BY
        sl.Date,
        shh.StartTime;

    DROP TABLE #TeacherScheduleClassBooks;
GO
