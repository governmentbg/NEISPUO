PRINT 'Create vwRegBookQualification'
GO

-- 3-78 Регистрационна книга за издадени документи за завършена степен на образование и за придобита професионална квалификация

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
        267       --3-114 Свидетелство за правоспособност
        --?       3-116 Свидетелство за правоспособност по заваряване
    )
GO
