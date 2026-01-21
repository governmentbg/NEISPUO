
CREATE VIEW refugee.RefugeesReceivedRejectedAdmissionByRegion
AS
SELECT
	ac.SchoolYear as SchoolYear,
	-- 'Учебна година',
	ac.Region as RegionName,
	-- 'Област',
	COUNT(ac.ChildNumber) ChildNumber ,
	-- 'Общо деца',
	SUM(ac.Rejected) as Rejected,
	--'От тях оттеглени',
	SUM(CASE WHEN ac.LastInstiutionType = 2 THEN 1 ELSE 0 END) as KindergartenLastInstitution,
	--'За ДГ',
	SUM(CASE WHEN ac.LastInstiutionType = 3 THEN 1 ELSE 0 END) as SchoolLastInstitution,
	-- 'За училище',
	ac.ClassOrGroup as ClassOrGroup,
	--'Випуск',
	SUM(CASE WHEN ac.PersonalIDType = 0 THEN 1 ELSE 0 END) as StudentsWithEGN,
	--'Деца с ЕГН',
	SUM(CASE WHEN ac.PersonalIDType = 1 THEN 1 ELSE 0 END) as StudentsWithLNCH ,
	-- 'Деца с ЛНЧ',
	SUM(CAST(ac.HasDocumentForCompletedClass AS INT)) as HasDocumentForCompletedClass,
	--,'Документи за обучение',
	SUM(ISNULL(CAST(ad.HasImmunizationStatusDocument AS INT), 0)) as HasImmunizationStatusDocumentSum,
	-- 'Документ за имунизации',
	SUM(CASE WHEN ac.PersonalIDType = 0 AND sc.ClassId < 0
			 THEN 1
			 ELSE 0
		END) as KGEnrolledWithEGN,
	-- 'Записани с ЕГН в ДГ',
	SUM(CASE WHEN ac.PersonalIDType = 1 AND sc.BasicClassId < 0
		 THEN 1
		 ELSE 0
		END) as KGEnrolledWithLNCH,
	--'Записани с ЛНЧ в ДГ',
	SUM(CASE WHEN ac.PersonalIDType = 0 AND sc.BasicClassId > 0
			 THEN 1
			 ELSE 0
		END) as SEnrolledWithEGN,
	-- 'Записани с ЕГН в училище',
	SUM(CASE WHEN ac.PersonalIDType = 1 AND sc.BasicClassId > 0
		 THEN 1
		 ELSE 0
		END) as SEnrolledWithLNCH,
	--'Записани с ЛНЧ в училище',
	MAX(ac.RegionID) as RegionID
	-- код по регион 
FROM
	refugee.v_ApplicationChildren ac
LEFT OUTER JOIN student.StudentClass sc ON
	sc.PersonId = ac.PersonId
LEFT OUTER JOIN inst_nom.ClassType ct ON
	sc.ClassTypeId = ct.ClassTypeID
	AND ct.ClassKind = 1
LEFT OUTER JOIN student.AdmissionDocument ad ON
	ad.id = sc.AdmissionDocumentId
GROUP BY
	ac.SchoolYear,
	ac.Region,
	ac.ClassOrGroup