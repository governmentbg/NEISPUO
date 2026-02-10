EXEC [school_books].[spCreateIdSequence] N'StudentSettings'
GO

CREATE TABLE [school_books].[StudentSettings] (
    [StudentSettingsId]         INT              NOT NULL,
    [PersonId]                  INT              NOT NULL,
    [AllowGradeEmails]          BIT              NOT NULL,
    [AllowAbsenceEmails]        BIT              NOT NULL,
    [AllowRemarkEmails]         BIT              NOT NULL,

    [Version]                   ROWVERSION       NOT NULL,

    CONSTRAINT [PK_StudentSettings] PRIMARY KEY ([StudentSettingsId]),

        -- external references
    CONSTRAINT [FK_StudentSettings_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),

    INDEX [IX__StudentSettingsPersonId] UNIQUE ([PersonId]),
);
GO
