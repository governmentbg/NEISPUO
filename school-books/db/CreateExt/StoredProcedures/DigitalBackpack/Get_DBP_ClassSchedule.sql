PRINT 'Create Get_DBP_ClassSchedule stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_DBP_ClassSchedule]
    @schoolYear INT,
    @classId INT,
    @fromDate DATE,
    @toDate DATE
AS
    SELECT
        [SchoolYear] = cb.SchoolYear,
        [InstitutionId] = cb.InstId,
        [ClassId] = cb.ClassId,
        [ScheduleLessonId] = sl.ScheduleLessonId,
        [ShiftId] = sch.ShiftId,
        [ShiftName] = sh.Name,
        [IsIndividualCurriculum] = sch.IsIndividualSchedule,
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
        [TeacherAzureIds] =
            IIF(tah.TeacherAbsenceId IS NULL,
                STRING_AGG(p.AzureID, ', ') WITHIN GROUP(ORDER BY p.AzureID),
                repl.AzureID),
        [TeacherNames] =
            IIF(tah.TeacherAbsenceId IS NULL,
                STRING_AGG(CONCAT_WS(' ', p.FirstName, p.LastName), ', ') WITHIN GROUP(ORDER BY p.AzureID),
                NULLIF(
                    CASE
                        WHEN repl.FirstName IS NOT NULL AND repl.LastName IS NOT NULL
                            THEN CONCAT_WS(' ', repl.FirstName, repl.LastName)
                        ELSE tah.ExtReplTeacherName
                    END,
                ''
                )
            ),
        [ReplTeacherIsNonSpecialist] = tah.ReplTeacherIsNonSpecialist,
        [IsEmptyHour] = IIF(tah.TeacherAbsenceId IS NOT NULL AND tah.ReplTeacherPersonId IS NULL AND tah.ExtReplTeacherName IS NULL, CAST(1 AS BIT), CAST(0 AS BIT))
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [school_books].[Schedule] sch ON cb.SchoolYear = sch.SchoolYear AND cb.ClassBookId = sch.ClassBookId
        INNER JOIN [school_books].[Shift] sh ON sch.SchoolYear = sh.SchoolYear AND sch.ShiftId = sh.ShiftId
        INNER JOIN [school_books].[ScheduleLesson] sl ON sch.SchoolYear = sl.SchoolYear AND sch.ScheduleId = sl.ScheduleId
        INNER JOIN [school_books].[ShiftHour] shh ON sl.SchoolYear = shh.SchoolYear AND sch.ShiftId = shh.ShiftId AND sl.Day = shh.Day AND sl.HourNumber = shh.HourNumber
        INNER JOIN [inst_year].[Curriculum] c ON sl.CurriculumId = c.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
        INNER JOIN [inst_year].[CurriculumTeacher] t ON c.CurriculumID = t.CurriculumID
        INNER JOIN [inst_basic].[StaffPosition] sp ON t.StaffPositionID = sp.StaffPositionID
        INNER JOIN [core].[Person] p ON sp.PersonID = p.PersonID
        LEFT JOIN [core].[Person] ics ON sch.PersonId = ics.PersonId
        LEFT JOIN [school_books].[TeacherAbsenceHour] tah ON sl.SchoolYear = tah.SchoolYear AND sl.ScheduleLessonId = tah.ScheduleLessonId
        LEFT JOIN [core].[Person] repl ON tah.ReplTeacherPersonId = repl.PersonID
    WHERE
        cb.SchoolYear = @schoolYear AND
        cb.ClassId = @classId AND
        sl.[Date] >= @fromDate AND
        sl.[Date] <= @toDate AND
        t.IsValid = 1 -- TODO add StaffPositionStartDate/StaffPositionTerminationDate validity check
    GROUP BY
        cb.SchoolYear,
        cb.InstId,
        cb.ClassId,
        sl.ScheduleLessonId,
        sch.ShiftId,
        sh.Name,
        sch.IsIndividualSchedule,
        ics.FirstName,
        ics.LastName,
        sl.Date,
        sl.Day,
        sl.HourNumber,
        shh.StartTime,
        shh.EndTime,
        c.CurriculumId,
        c.CurriculumGroupNum,
        s.SubjectID,
        s.SubjectName,
        s.SubjectNameShort,
        st.SubjectTypeID,
        st.Name,
        tah.TeacherAbsenceId,
        tah.ReplTeacherPersonId,
        tah.ReplTeacherIsNonSpecialist,
        tah.ExtReplTeacherName,
        repl.AzureID,
        repl.FirstName,
        repl.LastName
    ORDER BY
        sl.Date,
        sl.HourNumber
GO
