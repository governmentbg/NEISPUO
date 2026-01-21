PRINT 'Create vwRegBookCertificateDuplicateBasicDocument'
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
