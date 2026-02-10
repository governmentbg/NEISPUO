PRINT 'Create ClassBookExtProvider table'
GO

-- this table is in the school_books schema but is
-- maintained and modified by AdminSoft (institutions module)

CREATE TABLE [school_books].[ClassBookExtProvider](
    [SchoolYear]            SMALLINT    NOT NULL,
    [InstId]                INT         NOT NULL,
    [ExtSystemId]           INT         NULL,
    [ScheduleExtSystemId]   INT         NULL,

    PRIMARY KEY ([SchoolYear], [InstId]),
    CONSTRAINT [CHK_ExtSystemId_ScheduleExtSystemId] CHECK (
        NOT ([ExtSystemId] IS NOT NULL AND [ScheduleExtSystemId] IS NOT NULL)
    ),

    -- external references
    CONSTRAINT [FK_ClassBookExtProvider_InstId_SchoolYear]
        FOREIGN KEY ([InstId], [SchoolYear])
        REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_ClassBookExtProvider_ExtSystemId]
        FOREIGN KEY ([ExtSystemId])
        REFERENCES [core].[ExtSystem] ([ExtSystemID]),
    CONSTRAINT [FK_ClassBookExtProvider_ScheduleExtSystemId]
        FOREIGN KEY ([ScheduleExtSystemId])
        REFERENCES [core].[ExtSystem] ([ExtSystemID]),
)
GO

exec school_books.spDescTable  N'ClassBookExtProvider', N'Доставчик на електронен дневник за дадена институция/учебна година.'

exec school_books.spDescColumn N'ClassBookExtProvider', N'SchoolYear'           , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookExtProvider', N'InstId'               , N'Идентификатор на институция.'
exec school_books.spDescColumn N'ClassBookExtProvider', N'ExtSystemId'          , N'Идентификатор на доставчик на електронен дневник.'
exec school_books.spDescColumn N'ClassBookExtProvider', N'ScheduleExtSystemId'  , N'Идентификатор на доставчик на разписание за електронен дневник НЕИСПУО.'
