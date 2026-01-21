PRINT 'Create PersonnelSchoolBookAccess table'
GO

-- this table is in the school_books schema but is
-- maintained and modified by DSS (user management module)

CREATE TABLE [school_books].[PersonnelSchoolBookAccess](
    [RowID]             INT IDENTITY(1,1)   NOT NULL,
    [SchoolYear]        SMALLINT            NULL,
    [ClassBookID]       INT                 NULL,
    [PersonID]          INT                 NULL,
    [HasAdminAccess]    BIT                 NOT NULL,

    PRIMARY KEY ([RowID]),

    CONSTRAINT [FK_ClassBook] FOREIGN KEY([SchoolYear], [ClassBookID])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [FK_Person] FOREIGN KEY([PersonID])
        REFERENCES [core].[Person] ([PersonID]),

    INDEX [UQ_PersonnelSchoolBookAccess_SchoolYear_ClassBookID_PersonID]
        UNIQUE ([SchoolYear], [ClassBookID], [PersonID]) INCLUDE ([HasAdminAccess])
        WHERE
            [SchoolYear] IS NOT NULL AND
            [ClassBookID] IS NOT NULL AND
            [PersonID] IS NOT NULL
)
GO
