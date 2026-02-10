PRINT 'Create vwExtInstitution'
GO

CREATE OR ALTER VIEW [school_books].[vwExtInstitution]
AS
    SELECT
        i.InstitutionID,
        i.Name,
        CONVERT(NVARCHAR, i.InstitutionID) + ' - ' + i.Abbreviation + ' - (' + t.Name + ')' AS Abbreviation,
        t.TownID,
        t.Name AS TownName,
        m.MunicipalityID AS MunID,
        m.Name AS MunName,
        r.RegionID AS RegID,
        r.Name AS RegName
    FROM
        [core].[Institution] i
        INNER JOIN [inst_basic].[InstitutionDepartment] id on id.InstitutionID = i.InstitutionID
        INNER JOIN [location].[Town] t on id.TownID = t.TownID
        INNER JOIN [location].[Municipality] m on t.MunicipalityID = m.MunicipalityID
        INNER JOIN [location].[Region] r on m.RegionID = r.RegionID
    WHERE
        id.IsMain = 1
GO
