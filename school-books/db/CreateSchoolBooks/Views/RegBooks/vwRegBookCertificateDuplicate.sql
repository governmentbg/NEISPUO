PRINT 'Create vwRegBookCertificateDuplicate'
GO

-- 3-78Б Регистрационна книга за издадените дубликати на удостоверения

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
