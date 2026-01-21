PRINT 'Create Get_EduSurvey_Students stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_EduSurvey_Students]
    @schoolYear INT,
    @institutionId INT
AS
    SELECT
        p.PublicEduNumber,
        p.FirstName,
        p.MiddleName,
        p.LastName,
        Classes =
            STRING_AGG(CAST(cg.ClassName AS NVARCHAR(MAX)), ';')
            WITHIN GROUP (ORDER BY cg.BasicClassID, cg.ClassName),
        SPPOOSpecialityCodes =
            STRING_AGG(CAST(ss.SPPOOSpecialityCode AS NVARCHAR(MAX)), ';')
            WITHIN GROUP (ORDER BY cg.BasicClassID, cg.ClassName)
    FROM (
        SELECT DISTINCT cg.SchoolYear, cg.InstitutionID, cg.BasicClassID, cg.ClassName, sc.PersonID, sc.StudentSpecialityId
        FROM inst_year.ClassGroup cg
        JOIN inst_year.ClassGroup ccg ON cg.ClassID = ccg.ParentClassID
        JOIN inst_nom.ClassType ctype ON cg.ClassTypeID = ctype.ClassTypeID
        JOIN student.StudentClass sc
            ON ccg.SchoolYear = sc.SchoolYear
            AND ccg.InstitutionID = sc.InstitutionID
            AND ccg.ClassID = sc.ClassID
        WHERE cg.ParentClassID IS NULL
            AND (ctype.ClassKind IS NULL OR ctype.ClassKind = 1) -- Class
            AND cg.BasicClassID BETWEEN 8 AND 12
            AND sc.Status = 1 -- Enrolled
    ) cg
    JOIN core.Person p ON cg.PersonID = p.PersonID
    JOIN inst_nom.SPPOOSpeciality ss ON cg.StudentSpecialityId = ss.SPPOOSpecialityId
    WHERE cg.SchoolYear = @schoolYear AND cg.InstitutionID = @institutionId
    GROUP BY p.PublicEduNumber, p.FirstName, p.MiddleName, p.LastName
    ORDER BY p.PublicEduNumber, p.FirstName, p.MiddleName, p.LastName
GO
