GO
PRINT 'Insert Person'
GO

SET IDENTITY_INSERT core.Person ON
GO

-- SysUser persons
insert core.Person (PersonID,FirstName,MiddleName,LastName,PermanentAddress,PermanentTownID,CurrentAddress,CurrentTownID,PublicEduNumber,PersonalIDType,NationalityID,PersonalID,BirthDate,BirthPlaceTownID,BirthPlaceCountry,Gender,SchoolBooksCodesID,BirthPlace,AzureID)
select 1,N'Миграция',NULL,N'Миграция',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL UNION ALL
select 2,N'Интеграции',NULL,N'Интеграции',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL UNION ALL
select 3,N'Модул Дневници',NULL,N'Системен',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL UNION ALL
select 1012,N'МОН','','',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL UNION ALL
select 1013,N'Център за подкрепа на личностно развитие  - Сиела','','',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL UNION ALL
select 1014,N'Детска градина "Ханс Кристиан Андерсен"','','',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL UNION ALL
select 1015,N'Център за специална образователна подкрепа "Д-р Петър Берон"','','',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL UNION ALL
select 1016,N'Регионален център за подкрепа на процеса на приобщаващо образование - Пловдив','','',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL UNION ALL
select 1017,N'Детска градина "Звездичка"','','',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL UNION ALL
select 1018,N'ОУ "Св.Иван Рилски"','','',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL UNION ALL
select 1019,N'ЦПЛР Котел"','','',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL;

INSERT INTO core.Person(
    [PersonID],
    [FirstName],
    [MiddleName],
    [LastName],
    [PermanentAddress],
    [PermanentTownID],
    [CurrentAddress],
    [CurrentTownID],
    [PublicEduNumber],
    [PersonalIDType],
    [PersonalID],
    [BirthDate],
    [BirthPlaceTownID],
    [BirthPlaceCountry],
    [Gender],
    [AzureID]
)
SELECT
    p.PersonID,
    p.FirstName,
    p.MiddleName,
    p.LastName,
    PermanentAddress = CONCAT('PermanentAddress ', p.[Order]),
    PermanentTownID = 68134,
    CurrentAddress = CONCAT('CurrentAddress ', p.[Order]),
    CurrentTownID = 68134,
    PublicEduNumber = CAST(p.PersonID AS NVARCHAR),
    PersonalIDType = 0,
    PersonalID = p.EGN,
    p.BirthDate,
    BirthPlaceTownID = 68134,
    BirthPlaceCountry = 34,
    Gender,
    AzureID = CONCAT('AzureID_', p.PersonID)
FROM (
    SELECT
        p.PersonID,
        pn.FirstName,
        pn.MiddleName,
        pn.LastName,
        pn.EGN,
        pn.BirthDate,
        pn.Gender,
        pn.[Order]
    FROM
        (
            SELECT *, ROW_NUMBER() OVER (ORDER BY [PersonID]) as row_num
            FROM #tempPerson
        ) p
        INNER JOIN (
            SELECT *, ROW_NUMBER() OVER (ORDER BY [Order]) as row_num
            FROM #tempPersonNames
        ) pn ON p.row_num = pn.row_num
) p

SET IDENTITY_INSERT core.Person OFF
GO
