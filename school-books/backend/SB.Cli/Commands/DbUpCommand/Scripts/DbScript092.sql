CREATE TABLE [school_books].[ClassBookExtProvider](
    [SchoolYear]    SMALLINT            NOT NULL,
    [InstId]        INT                 NOT NULL,
    [ExtSystemId]   INT                 NULL,

    PRIMARY KEY ([SchoolYear], [InstId]),

    CONSTRAINT [FK_ClassBookExtProvider_InstId_SchoolYear]
        FOREIGN KEY ([InstId], [SchoolYear])
        REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_ClassBookExtProvider_ExtSystemId]
        FOREIGN KEY ([ExtSystemId])
        REFERENCES [core].[ExtSystem] ([ExtSystemID]),
)
GO

INSERT INTO [school_books].[ClassBookExtProvider] (SchoolYear, InstId, ExtSystemId)
SELECT
    2022,
    InstitutionId,
    CBExtProviderId
FROM [core].[InstitutionConfData]
WHERE SchoolYear IN (2022, 2023)

INSERT INTO [school_books].[ClassBookExtProvider] (SchoolYear, InstId, ExtSystemId)
SELECT
    SchoolYear,
    InstitutionId,
    CBExtProviderId
FROM [core].[InstitutionConfData]
WHERE SchoolYear = 2023
GO
