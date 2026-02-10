PRINT 'Create Get_EduSurvey_Teachers stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_EduSurvey_Teachers]
    @schoolYear INT,
    @institutionId INT
AS
    WITH Teachers AS (
        SELECT
            ct.SchoolYear,
            ct.InstitutionID,
            ct.PersonID,
            SubjectSubjectTypes =
                STRING_AGG(CAST(ct.SubjectID AS NVARCHAR(MAX)) + '/' + CAST(ct.SubjectTypeID AS NVARCHAR(MAX)), ';')
                WITHIN GROUP (ORDER BY ct.SubjectID, ct.SubjectTypeID)
        FROM (
            SELECT DISTINCT c.SchoolYear, c.InstitutionID, sp.PersonID, c.SubjectID, c.SubjectTypeID
            FROM inst_year.Curriculum c
            JOIN inst_year.CurriculumTeacher ct ON c.CurriculumID = ct.CurriculumID
            JOIN inst_basic.StaffPosition sp ON ct.StaffPositionID = sp.StaffPositionID
        ) ct
        GROUP BY ct.SchoolYear, ct.InstitutionID, ct.PersonID
    ),
    Groups AS (
        SELECT
            cg.SchoolYear,
            cg.InstitutionID,
            cg.PersonID,
            cg.NKPDPositionID,
            Classes =
                STRING_AGG(CAST(cg.ClassName AS NVARCHAR(MAX)), ';')
                WITHIN GROUP (ORDER BY cg.BasicClassID, cg.ClassName)
        FROM (
            SELECT distinct cg.SchoolYear, cg.InstitutionID, cg.BasicClassID, cg.ClassName, sp.PersonID, sp.NKPDPositionID
            FROM inst_year.ClassGroup cg
            JOIN inst_nom.ClassType ctype ON cg.ClassTypeID = ctype.ClassTypeID
            JOIN inst_year.CurriculumClass cc ON cg.ClassID = cc.ClassID
            JOIN inst_year.CurriculumTeacher ct ON cc.CurriculumID = ct.CurriculumID
            JOIN inst_basic.StaffPosition sp ON ct.StaffPositionID = sp.StaffPositionID
            WHERE cg.ParentClassID IS NULL
                AND (ctype.ClassKind IS NULL OR ctype.ClassKind = 1) -- Class
        ) cg
        GROUP BY cg.SchoolYear, cg.InstitutionID, cg.PersonID, cg.NKPDPositionID
    )
    SELECT
        p.PublicEduNumber,
        p.FirstName,
        p.MiddleName,
        p.LastName,
        cg.NKPDPositionID,
        cg.Classes,
        ct.SubjectSubjectTypes
    FROM Teachers ct
    JOIN Groups cg
        ON ct.SchoolYear = cg.SchoolYear
        AND ct.InstitutionID = cg.InstitutionID
        AND ct.PersonID = cg.PersonID
    JOIN core.Person p ON ct.PersonID = p.PersonID
    WHERE ct.SchoolYear = @schoolYear AND ct.InstitutionID = @institutionId
    ORDER BY p.PublicEduNumber, p.FirstName, p.MiddleName, p.LastName
GO
