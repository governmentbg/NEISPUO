GO
PRINT 'Insert HisMedicalNotice'
GO

INSERT INTO [school_books].[HisMedicalNoticeBatch] (CreateDate, RequestId)
VALUES (
    GETDATE(),
    N'0HHHHHH000000:00000001'
)

DECLARE @hisMedicalNoticeBatchId INT = SCOPE_IDENTITY();

INSERT INTO [school_books].[HisMedicalNotice] (
    HisMedicalNoticeBatchId,
    PersonalIdTypeId,
    CreateDate,
    NrnMedicalNotice,
    NrnExamination,
    IdentifierType,
    Identifier,
    GivenName,
    FamilyName,
    Pmi,
    FromDate,
    ToDate,
    AuthoredOn
) VALUES (
    @hisMedicalNoticeBatchId,
    0,
    GETDATE(),
    '23235B100001',
    '23032B100001',
    1,
    '3904073451',
    N'Румен',
    N'Станчев',
    N'2300011922',
    '2025-09-22',
    '2025-09-22',
    GETDATE()
)

DECLARE @hisMedicalNoticeId INT = SCOPE_IDENTITY();
INSERT INTO [school_books].[HisMedicalNoticeSchoolYear] (
    HisMedicalNoticeId,
    SchoolYear
) VALUES (
    @hisMedicalNoticeId,
    2024
), (
    @hisMedicalNoticeId,
    2025
)

INSERT INTO [school_books].[HisMedicalNotice] (
    HisMedicalNoticeBatchId,
    PersonalIdTypeId,
    CreateDate,
    NrnMedicalNotice,
    NrnExamination,
    IdentifierType,
    Identifier,
    GivenName,
    FamilyName,
    Pmi,
    FromDate,
    ToDate,
    AuthoredOn
) VALUES (
    @hisMedicalNoticeBatchId,
    0,
    GETDATE(),
    '23235B100002',
    '23032B100002',
    1,
    '4108230232',
    N'Методи',
    N'Станев',
    N'2300011922',
    '2025-10-15',
    '2025-10-20',
    GETDATE()
)

SET @hisMedicalNoticeId = SCOPE_IDENTITY();
INSERT INTO [school_books].[HisMedicalNoticeSchoolYear] (
    HisMedicalNoticeId,
    SchoolYear
) VALUES (
    @hisMedicalNoticeId,
    2025
)

INSERT INTO [school_books].[HisMedicalNotice] (
    HisMedicalNoticeBatchId,
    PersonalIdTypeId,
    CreateDate,
    NrnMedicalNotice,
    NrnExamination,
    IdentifierType,
    Identifier,
    GivenName,
    FamilyName,
    Pmi,
    FromDate,
    ToDate,
    AuthoredOn
) VALUES (
    @hisMedicalNoticeBatchId,
    0,
    GETDATE(),
    '23235B100003',
    '23032B100003',
    1,
    '9912187047',
    N'Асен',
    N'Йорданов',
    N'2300011922',
    '2025-10-01',
    '2025-10-15',
    GETDATE()
)

SET @hisMedicalNoticeId = SCOPE_IDENTITY();
INSERT INTO [school_books].[HisMedicalNoticeSchoolYear] (
    HisMedicalNoticeId,
    SchoolYear
) VALUES (
    @hisMedicalNoticeId,
    2025
)
