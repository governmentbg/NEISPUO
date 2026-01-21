DROP TABLE [school_books].[RegBookCertificateDuplicate]
DROP TABLE [school_books].[RegBookCertificate]
DROP TABLE [school_books].[RegBookQualificationDuplicate]
DROP TABLE [school_books].[RegBookQualification]
GO

CREATE OR ALTER VIEW [school_books].[vwRegBookCertificate]
AS
    SELECT
        Id
        ,InstitutionId
        ,SchoolYear
        ,RegistrationNumberTotal -- [Пореден рег. №]                            -- Number
        ,RegistrationNumberYear  -- [Рег. № за годината]                        -- RegNumber
        ,RegistrationDate        -- [Дата на издаване]                          -- RegDate
        ,PersonId                -- [Собствено, бащино и фамилно име | ЕГН/ЛНЧ] -- PersonId
        ,BasicDocumentId         -- [Вид документ]                              -- BasicDocumentId
        ,EduFormId               -- [Форма на обучение]                         -- EduFormId
        ,Series                  -- [Серия]                                     -- DocumentSeries
        ,FactoryNumber           -- [Фабричен номер]                            -- DocumentNumber
    FROM document.Diploma
    WHERE BasicDocumentId IN (
        93,   --3-37 Удостоверение за професионално обучение
        37,   --3-102 Удостоверение за валидиране на компетентности по учебен предмет, невключен в дипломата за средно образование
        38,   --3-27В Удостоверение за валидиране на компетентности за начален или първи гимназиален етап/основна степен на образование
        193   --3-37В Удостоверение за валидиране на професионална квалификация по част от професия
        -- ?    3-19 Удостоверение за задължително предучилищно образование
        -- ?    3-21 Удостоверение за завършен първи клас
        -- ?    3-23 Удостоверение за завършен клас от началния етап на основно образование (II клас и III клас)
        -- ?    3-25 Удостоверение за завършен начален етап на основно образование – IV клас
        -- ?    3-103 Удостоверение за завършен клас
        -- ?    ОГД Удостоверение за валидиране на компетентности за клас от  основната степен на образование/първи гимназиален етап
        -- ?    ОГД Удостоверение за валидиране на компетентности по учебен предмет за един или няколко класа
        -- ?    ОГД Удостоверение за проведено обучение български език и литература, история и цивилизации и география и икономика
        -- ?    ОГД Удостоверение за признат  завършен гимназиален етап/ средно образование и/или професионална квалификация
        -- ?    ОГД Удостоверение за признат учебен срок, клас/класове или основно образование
        -- ?    ОГД Уверение
    )
GO

CREATE OR ALTER VIEW [school_books].[vwRegBookCertificateDuplicate]
AS
    SELECT
        d.Id
        ,d.InstitutionId
        ,d.SchoolYear
        ,d.RegistrationNumberTotal                                -- [Пореден рег. №]                                   -- Number
        ,d.RegistrationNumberYear                                 -- [Рег. № за годината]                               -- RegNumber
        ,d.RegistrationDate                                       -- [Дата на издаване]                                 -- RegDate
        ,d.PersonId                                               -- [Собствено, бащино и фамилно име | ЕГН/ЛНЧ]        -- PersonId
        ,d.BasicDocumentId                                        -- [Вид на издадения дубликат]                        -- BasicDocumentId
        ,d.OriginalDiplomaId
        ,o.Id AS OrigId
        ,o.RegistrationNumberYear AS OrigRegistrationNumberYear    -- [Рег. № на оригиналното удостоверение]             -- missing
        ,o.RegistrationDate AS OrigRegistrationDate                -- [Дата на издаване на оригиналното удостоверение]   -- missing
    FROM document.Diploma d
    LEFT JOIN document.Diploma o ON d.OriginalDiplomaId = d.Id
    WHERE d.BasicDocumentId IN (
        39  -- 3-27аВ Дубликат на удостоверение за валидиране на компетентности за начален или първи гимназиален етап/основна степен на образование
    )
GO

CREATE OR ALTER VIEW [school_books].[vwRegBookQualification]
AS
    SELECT
        Id
        ,InstitutionId
        ,SchoolYear
        ,BasicDocumentId                                                                            -- BasicDocumentId
        ,RegistrationNumberTotal -- [Рег. № по реда за издаване на документа]                       -- Number
        ,RegistrationNumberYear  -- [Рег. № по ред на годината]                                     -- RegNumber
        ,RegistrationDate        -- [Дата]                                                          -- RegDate
        ,PersonId                -- [Собствено, бащино и фамилно име на ученика | ЕГН/ЛНЧ]          -- PersonId
        ,EduFormId               -- [Форма на обучение]                                             -- EduFormId
        ,SPPOOSpecialityId       -- [Общообразователна подготовка / профил, професия, специалност]  -- ClassSpecialityId
        ,GPA                                                                                        -- GPA
        ,GPAText                 -- [Общ успех]                                                     --
        ,Series                  -- [Серия на документа]                                            -- DocumentSeries
        ,FactoryNumber           -- [№ на документа]                                                -- DocumentNumber
    FROM document.Diploma
    WHERE BasicDocumentId IN (
        133,   -- 3-20 Свидетелство за основно образование (за минали години)
        142,   -- 3-30 Свидетелство за основно образование
        14,    -- 3-31 Удостоверение за завършен първи гимназиален етап на средно образование
        262,   -- 3-32 Удостоверение за завършен втори гимназиален етап на средно образование
        44,    -- 3-22 Удостоверение за завършен гимназиален етап
        253,   -- 3-44 Диплома за средно образование
        17,    -- 3-54 Свидетелство за професионална квалификация
        194,   -- 3-54В Свидетелство за валидиране на професионална квалификация
        35,    -- 3-34 Диплома за средно образование
        36     -- 3-42 Диплома за средно образование
        --?       3-114 Свидетелство за правоспособност
        --?       3-116 Свидетелство за правоспособност по заваряване
    )
GO

CREATE OR ALTER VIEW [school_books].[vwRegBookQualificationDuplicate]
AS
    SELECT
        d.Id
        ,d.InstitutionId
        ,d.SchoolYear
        ,d.RegistrationNumberTotal                                -- [Рег. № по реда на издаване на дубликата]                       -- Number
        ,d.RegistrationNumberYear                                 -- [Рег. № по ред за годината на издаване на дубликата]            -- RegNumber
        ,d.RegistrationDate                                       -- [Дата на регистриране на дубликата]                             -- RegDate
        ,d.PersonId                                               -- [Трите имена на притежателя на дубликата | ЕГН/ЛНЧ]             -- PersonId
        ,d.BasicDocumentId                                        -- [Вид на издадения дубликат]                                     -- BasicDocumentId
        ,d.Series                                                 -- [Серия на документа]                                            -- DocumentSeries
        ,d.FactoryNumber                                          -- [№ на бланката на дубликата]                                    -- DocumentNumber
        ,d.SPPOOSpecialityId                                      -- [Общообразователна подготовка / профил, професия, специалност]  -- ClassSpecialityId
        ,d.EduFormId                                              -- [Форма на обучение]                                             -- EduFormId
        ,d.OriginalDiplomaId
        ,o.Id AS OrigId
        ,o.Series AS OrigSeries                                    -- [Серия на бланката на оригинала на документа]                   -- OrigDocumentNumber
        ,o.FactoryNumber AS OrigFactoryNumber                      -- [№ на бланката на оригинала на документа]                       -- OrigDocumentSeries
        ,o.RegistrationNumberYear AS OrigRegistrationNumberYear    -- [Рег. № на оригиналния издаден документ]                        -- OrigDocumentRegNumber
        ,o.RegistrationDate AS OrigRegistrationDate                -- [Дата на издаване на оригинала]                                 -- OrigDocumentRegDate
    FROM document.Diploma d
    LEFT JOIN document.Diploma o ON d.OriginalDiplomaId = d.Id
    WHERE d.BasicDocumentId IN (
        113,  -- 3-30а Дубликат на свидетелство за основно образование
        15,   -- 3-31а Дубликат на удостоверение за завършен първи гимназиален етап на средно образование
        263,  -- 3-32A Дубликат на удостоверение за завършен втори гимназиален етап на средна образование
        45,   -- 3-22а Дубликат на удостоверение за завършен гимназиален етап
        31,   -- 3-44а Дубликат на диплома за средно образование
        195   -- 3-54аВ Дубликат на свидетелство за валидиране на професионална квалификация
    )
GO
