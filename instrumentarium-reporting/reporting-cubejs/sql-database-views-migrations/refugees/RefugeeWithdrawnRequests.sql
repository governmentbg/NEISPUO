CREATE VIEW [refugee].[RefugeeWithdrawnRequests] -- [Оттеглени заявления от търсещи или получили закрила]
AS
SELECT
	ac.SchoolYear AS SchoolYear,
	--'Учебна година',
	ac.Town as TownName,
	-- 'Населено място',
	ac.Municipality as MunicipalityName,
	--'Община',
	ac.Region as RegionName,
	-- 'Област',
	ac.ApplicationNumber as ApplicationNumber,
	---'Номер на заявление',
	CONVERT(VARCHAR(10), ac.CreateDate, 104) as CreateDate,
	-- 'Дата на заявление',
	g.Name as PersonalIDTypeName,
	-- 'Вид идентификатор',
	ac.PersonalIDType as PersonalIDType,
	--'ЕГН/ЛНЧ',
	ac.FirstName as FirstName,
	---'Име',
	ISNULL(ac.MiddleName, '') as MiddleName,
	-- 'Презиме',
	ac.LastName as LastName,
	-- 'Фамилия',
	CASE
		WHEN ac.LastBasicClassId IN (-3, -4, -5) THEN 'ДГ'
		WHEN ac.LastBasicClassId IN (-1, -6) THEN 'Подготвителна'
		ELSE ISNULL(
			CAST(ac.LastBasicClassId AS VARCHAR(25)),
			'Не е посочено'
		)
	END AS ClassOrGroup,
	-- 'Випуск',
	CONVERT(VARCHAR(10), ac.CancellationDate, 104) as CancellationDate,
	--- 'Дата на оттеляне',
	ac.CancellationReason,
	--'Причина за оттегляне',
	ac.TownID,
	ac.MunicipalityID,
	ac.RegionID
FROM
	refugee.v_ApplicationChildren ac
	INNER JOIN noms.Gender g ON g.GenderID = ac.PersonalIDType
WHERE
	ac.Status = 2 --оттеглено
	-- inst_basic.ЧасовеДОБмигранти source