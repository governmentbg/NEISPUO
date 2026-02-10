PRINT 'Create vwRegBookQualificationDuplicate'
GO

-- 3-73 Регистрационна книга за издадени дубликати на документите за завършена степен на образование и за придобита професионална квалификация

CREATE OR ALTER VIEW [school_books].[vwRegBookQualificationDuplicate]
WITH SCHEMABINDING
AS
    SELECT
        d.Id
        ,d.InstitutionId
        ,d.SchoolYear
        ,d.RegistrationNumberTotal                                  -- [Рег. № по реда на издаване на дубликата]
        ,d.RegistrationNumberYear                                   -- [Рег. № по ред за годината на издаване на дубликата]
        ,d.RegistrationDate                                         -- [Дата на регистриране на дубликата]
        ,d.PersonId                                                 -- [Трите имена на притежателя на дубликата | ЕГН/ЛНЧ]
        ,d.BasicDocumentId                                          -- [Вид на издадения дубликат]
        ,d.Series                                                   -- [Серия на документа]
        ,d.FactoryNumber                                            -- [№ на бланката на дубликата]
        -- [Общообразователна подготовка / профил, професия, специалност]
        -- показват се и трите, конкатенирани - ClassTypeId, SPPOOProfessionId и SPPOOSpecialityId
		,d.ClassTypeId                                              -- [Общообразователна подготовка / профил]
		,d.SPPOOProfessionId                                        -- [Професия]
		,d.SPPOOSpecialityId                                        -- [Специалност]
        ,d.EduFormId                                                -- [Форма на обучение]
        ,dad.Series AS OrigSeries                                   -- [Серия на бланката на оригинала на документа]
        ,dad.FactoryNumber AS OrigFactoryNumber                     -- [№ на бланката на оригинала на документа]
        ,dad.RegistratioNumber AS OrigRegistrationNumber            -- [Рег. № на оригиналния издаден документ]
        ,dad.RegistrationNumberYear AS OrigRegistrationNumberYear   -- [Рег. № на оригиналния издаден документ за годината]
        ,dad.RegistrationDate AS OrigRegistrationDate               -- [Дата на издаване на оригинала]
        ,d.IsCancelled                                              -- [Анулиран]
    FROM document.Diploma d
    JOIN document.BasicDocument bd ON d.BasicDocumentId = bd.Id
    LEFT JOIN document.DiplomaAdditionalDocument dad on dad.DiplomaId = d.Id
    WHERE d.BasicDocumentId IN (
        113   -- 3-30а Дубликат на свидетелство за основно образование
        ,15   -- 3-31а Дубликат на удостоверение за завършен първи гимназиален етап на средно образование
        ,263  -- 3-32A Дубликат на удостоверение за завършен втори гимназиален етап на средна образование
        ,45   -- 3-22а Дубликат на удостоверение за завършен гимназиален етап
        ,31   -- 3-44а Дубликат на диплома за средно образование
        ,195  -- 3-54аВ Дубликат на свидетелство за валидиране на професионална квалификация
        ,94   -- 3-54а Дубликат на свидетелство за професионална квалификация
        ,281  -- 3-114a Дубликат на свидетелство за правоспособност
    )
    AND (bd.IsIncludedInRegister = 0 OR d.IsSigned = 1)
GO
