GO

CREATE OR ALTER VIEW [school_books].[vwRegBookCertificate]
WITH SCHEMABINDING
AS
    SELECT
        d.Id
        ,d.InstitutionId
        ,d.SchoolYear
        ,d.RegistrationNumberTotal  -- [Пореден рег. №]
        ,d.RegistrationNumberYear   -- [Рег. № за годината]
        ,d.RegistrationDate         -- [Дата на издаване]
        ,d.PersonId                 -- [Собствено, бащино и фамилно име | ЕГН/ЛНЧ]
        ,d.BasicDocumentId          -- [Вид документ]
        ,d.EduFormId                -- [Форма на обучение]
        ,d.Series                   -- [Серия]
        ,d.FactoryNumber            -- [Фабричен номер]
        ,d.IsCancelled              -- [Анулиран]
    FROM document.Diploma d
    JOIN document.BasicDocument bd ON d.BasicDocumentId = bd.Id
    WHERE BasicDocumentId IN (
        93    --3-37 Удостоверение за професионално обучение
        ,37   --3-102 Удостоверение за валидиране на компетентности по учебен предмет, невключен в дипломата за средно образование
        ,193  --3-37В Удостоверение за валидиране на професионална квалификация по част от професия
        ,264  --3-103 Удостоверение за завършен клас
        -- ?    3-19 Удостоверение за задължително предучилищно образование
        -- ?    3-21 Удостоверение за завършен първи клас
        -- ?    3-23 Удостоверение за завършен клас от началния етап на основно образование (II клас и III клас)
        -- ?    3-25 Удостоверение за завършен начален етап на основно образование – IV клас
        -- ?    3-27В Удостоверение за валидиране на компетентности за начален или първи гимназиален етап/основна степен на образование
    )
    AND (bd.IsIncludedInRegister = 0 OR d.IsSigned = 1)

    UNION ALL

    SELECT
        Id
        ,InstitutionId
        ,SchoolYear
        ,RegNumberTotal     AS RegistrationNumberTotal
        ,RegNumber          AS RegistrationNumberYear
        ,IssueDate          AS RegistrationDate
        ,PersonId
        ,BasicDocumentId
        ,NULL               AS EduFormId
        ,Series
        ,FactoryNumber
        ,CAST(0 AS BIT)     AS IsCancelled
    FROM student.OtherDocument
    WHERE BasicDocumentId IN (
        264   --3-103 Удостоверение за завършен клас
        -- ?    3-19 Удостоверение за задължително предучилищно образование
        -- ?    3-21 Удостоверение за завършен първи клас
        -- ?    3-23 Удостоверение за завършен клас от началния етап на основно образование (II клас и III клас)
        -- ?    3-25 Удостоверение за завършен начален етап на основно образование – IV клас
        -- ?    3-27В Удостоверение за валидиране на компетентности за начален или първи гимназиален етап/основна степен на образование
        -- ?    ОГД Удостоверение за валидиране на компетентности за клас от  основната степен на образование/първи гимназиален етап
        -- ?    ОГД Удостоверение за валидиране на компетентности по учебен предмет за един или няколко класа
        -- ?    ОГД Удостоверение за проведено обучение български език и литература, история и цивилизации и география и икономика
        -- ?    ОГД Удостоверение за признат  завършен гимназиален етап/ средно образование и/или професионална квалификация
        -- ?    ОГД Удостоверение за признат учебен срок, клас/класове или основно образование
        -- ?    ОГД Уверение
    )
GO

CREATE OR ALTER VIEW [school_books].[vwRegBookCertificateBasicDocument]
WITH SCHEMABINDING
AS
    SELECT
        Id
        ,Name
    FROM document.BasicDocument
    WHERE Id IN (
        93    --3-37 Удостоверение за професионално обучение
        ,37   --3-102 Удостоверение за валидиране на компетентности по учебен предмет, невключен в дипломата за средно образование
        ,193  --3-37В Удостоверение за валидиране на професионална квалификация по част от професия
        ,264  --3-103 Удостоверение за завършен клас
        -- ?    3-19 Удостоверение за задължително предучилищно образование
        -- ?    3-21 Удостоверение за завършен първи клас
        -- ?    3-23 Удостоверение за завършен клас от началния етап на основно образование (II клас и III клас)
        -- ?    3-25 Удостоверение за завършен начален етап на основно образование – IV клас
        -- ?    3-27В Удостоверение за валидиране на компетентности за начален или първи гимназиален етап/основна степен на образование


        ,264  --3-103 Удостоверение за завършен клас
        -- ?    3-19 Удостоверение за задължително предучилищно образование
        -- ?    3-21 Удостоверение за завършен първи клас
        -- ?    3-23 Удостоверение за завършен клас от началния етап на основно образование (II клас и III клас)
        -- ?    3-25 Удостоверение за завършен начален етап на основно образование – IV клас
        -- ?    3-27В Удостоверение за валидиране на компетентности за начален или първи гимназиален етап/основна степен на образование
        -- ?    ОГД Удостоверение за валидиране на компетентности за клас от  основната степен на образование/първи гимназиален етап
        -- ?    ОГД Удостоверение за валидиране на компетентности по учебен предмет за един или няколко класа
        -- ?    ОГД Удостоверение за проведено обучение български език и литература, история и цивилизации и география и икономика
        -- ?    ОГД Удостоверение за признат  завършен гимназиален етап/ средно образование и/или професионална квалификация
        -- ?    ОГД Удостоверение за признат учебен срок, клас/класове или основно образование
        -- ?    ОГД Уверение
    )
GO

CREATE OR ALTER VIEW [school_books].[vwRegBookCertificateDuplicate]
WITH SCHEMABINDING
AS
    SELECT
        d.Id
        ,d.InstitutionId
        ,d.SchoolYear
        ,d.RegistrationNumberTotal                                  -- [Пореден рег. №]
        ,d.RegistrationNumberYear                                   -- [Рег. № за годината]
        ,d.RegistrationDate                                         -- [Дата на издаване]
        ,d.PersonId                                                 -- [Собствено, бащино и фамилно име | ЕГН/ЛНЧ]
        ,d.BasicDocumentId                                          -- [Вид на издадения дубликат]
        ,dad.RegistratioNumber AS OrigRegistrationNumber            -- [Рег. № на оригиналното удостоверение]
        ,dad.RegistrationNumberYear AS OrigRegistrationNumberYear   -- [Рег. № на оригиналното удостоверение за годината]
        ,dad.RegistrationDate AS OrigRegistrationDate               -- [Дата на издаване на оригинала]
        ,d.IsCancelled                                              -- [Анулиран]
    FROM document.Diploma d
    JOIN document.BasicDocument bd ON d.BasicDocumentId = bd.Id
    LEFT JOIN document.DiplomaAdditionalDocument dad on dad.DiplomaId = d.Id
    WHERE d.BasicDocumentId IN (
        39  -- 3-27аВ Дубликат на удостоверение за валидиране на компетентности за начален или първи гимназиален етап/основна степен на образование
    )
    AND (bd.IsIncludedInRegister = 0 OR d.IsSigned = 1)
GO

CREATE OR ALTER VIEW [school_books].[vwRegBookCertificateDuplicateBasicDocument]
WITH SCHEMABINDING
AS
    SELECT
        Id
        ,Name
    FROM document.BasicDocument
    WHERE Id IN (
        39  -- 3-27аВ Дубликат на удостоверение за валидиране на компетентности за начален или първи гимназиален етап/основна степен на образование
    )
GO

CREATE OR ALTER VIEW [school_books].[vwRegBookQualification]
WITH SCHEMABINDING
AS
    SELECT
        d.Id
        ,d.InstitutionId
        ,d.SchoolYear
        ,d.BasicDocumentId
        ,d.RegistrationNumberTotal                                      -- [Рег. № по реда за издаване на документа]
        ,d.RegistrationNumberYear                                       -- [Рег. № по ред на годината]
        ,d.RegistrationDate                                             -- [Дата]
        ,d.PersonId                                                     -- [Собствено, бащино и фамилно име на ученика | ЕГН/ЛНЧ]
        ,d.EduFormId                                                    -- [Форма на обучение]
        -- [Общообразователна подготовка / профил, професия, специалност]
        -- показват се и трите, конкатенирани - ClassTypeId, SPPOOProfessionId и SPPOOSpecialityId
        ,d.ClassTypeId                                                  -- [Общообразователна подготовка / профил]
        ,d.SPPOOProfessionId                                            -- [Професия]
        ,d.SPPOOSpecialityId                                            -- [Специалност]
        ,CONCAT_WS(' ', d.GPAText, IIF(d.GPA > 0, d.GPA, NULL)) AS GPA  -- [Общ успех]
        ,d.Series                                                       -- [Серия на документа]
        ,d.FactoryNumber                                                -- [№ на документа]
        ,d.IsCancelled                                                  -- [Анулиран]
    FROM document.Diploma d
    JOIN document.BasicDocument bd ON d.BasicDocumentId = bd.Id
    WHERE BasicDocumentId IN (
        133    -- 3-20 Свидетелство за основно образование (за минали години)
        ,142   -- 3-30 Свидетелство за основно образование
        ,14    -- 3-31 Удостоверение за завършен първи гимназиален етап на средно образование
        ,262   -- 3-32 Удостоверение за завършен втори гимназиален етап на средно образование
        ,44    -- 3-22 Удостоверение за завършен гимназиален етап
        ,253   -- 3-44 Диплома за средно образование
        ,17    -- 3-54 Свидетелство за професионална квалификация
        ,194   -- 3-54В Свидетелство за валидиране на професионална квалификация
        ,35    -- 3-34 Диплома за средно образование
        ,36    -- 3-42 Диплома за средно образование

    )
    AND (bd.IsIncludedInRegister = 0 OR d.IsSigned = 1)

    UNION ALL

    SELECT
        Id
        ,InstitutionId
        ,SchoolYear
        ,BasicDocumentId
        ,RegNumberTotal     AS RegistrationNumberTotal
        ,RegNumber          AS RegistrationNumberYear
        ,IssueDate          AS RegistrationDate
        ,PersonId
        ,NULL               AS EduFormId
        ,NULL               AS ClassTypeId
        ,NULL               AS SPPOOProfessionId
        ,NULL               AS SPPOOSpecialityId
        ,''                 AS GPA
        ,Series
        ,FactoryNumber
        ,CAST(0 AS BIT)     AS IsCancelled
    FROM student.OtherDocument
    WHERE BasicDocumentId IN (
        -1
        --?       3-114 Свидетелство за правоспособност
        --?       3-116 Свидетелство за правоспособност по заваряване
    )
GO

CREATE OR ALTER VIEW [school_books].[vwRegBookQualificationBasicDocument]
WITH SCHEMABINDING
AS
    SELECT
        Id
        ,Name
    FROM document.BasicDocument
    WHERE Id IN (
        133    -- 3-20 Свидетелство за основно образование (за минали години)
        ,142   -- 3-30 Свидетелство за основно образование
        ,14    -- 3-31 Удостоверение за завършен първи гимназиален етап на средно образование
        ,262   -- 3-32 Удостоверение за завършен втори гимназиален етап на средно образование
        ,44    -- 3-22 Удостоверение за завършен гимназиален етап
        ,253   -- 3-44 Диплома за средно образование
        ,17    -- 3-54 Свидетелство за професионална квалификация
        ,194   -- 3-54В Свидетелство за валидиране на професионална квалификация
        ,35    -- 3-34 Диплома за средно образование
        ,36    -- 3-42 Диплома за средно образование

        --?       3-114 Свидетелство за правоспособност
        --?       3-116 Свидетелство за правоспособност по заваряване
    )
GO

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
    )
    AND (bd.IsIncludedInRegister = 0 OR d.IsSigned = 1)
GO

CREATE OR ALTER VIEW [school_books].[vwRegBookQualificationDuplicateBasicDocument]
WITH SCHEMABINDING
AS
    SELECT
        Id
        ,Name
    FROM document.BasicDocument
    WHERE Id IN (
        113   -- 3-30а Дубликат на свидетелство за основно образование
        ,15   -- 3-31а Дубликат на удостоверение за завършен първи гимназиален етап на средно образование
        ,263  -- 3-32A Дубликат на удостоверение за завършен втори гимназиален етап на средна образование
        ,45   -- 3-22а Дубликат на удостоверение за завършен гимназиален етап
        ,31   -- 3-44а Дубликат на диплома за средно образование
        ,195  -- 3-54аВ Дубликат на свидетелство за валидиране на професионална квалификация
        ,94   -- 3-54а Дубликат на свидетелство за професионална квалификация
    )
GO
