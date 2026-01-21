PRINT 'Create GradeChangeExamsAdmProtocolCommissioner table'
GO

CREATE TABLE [school_books].[GradeChangeExamsAdmProtocolCommissioner] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [GradeChangeExamsAdmProtocolId]     INT              NOT NULL,
    [PersonId]                          INT              NOT NULL,

    [IsChairman]                        BIT              NOT NULL,
    [OrderNum]                          SMALLINT         NOT NULL,

    CONSTRAINT [PK_GradeChangeExamsAdmProtocolCommissioner] PRIMARY KEY ([SchoolYear], [GradeChangeExamsAdmProtocolId], [PersonId]),
    CONSTRAINT [FK_GradeChangeExamsAdmProtocolCommissioner_GradeChangeExamsAdmProtocol] FOREIGN KEY ([SchoolYear], [GradeChangeExamsAdmProtocolId])
        REFERENCES [school_books].[GradeChangeExamsAdmProtocol] ([SchoolYear], [GradeChangeExamsAdmProtocolId]),

    -- external references
    CONSTRAINT [FK_GradeChangeExamsAdmProtocolCommissioner_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
);
GO

exec school_books.spDescTable  N'GradeChangeExamsAdmProtocolCommissioner', N'Протокол за допускане до изпити за промяна на оценката - член на комисията.'

exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolCommissioner', N'SchoolYear'                         , N'Учебна година.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolCommissioner', N'GradeChangeExamsAdmProtocolId'      , N'Идентификатор на протокол за допускане до изпити за промяна на оценката.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolCommissioner', N'PersonId'                           , N'Идентификатор на член на комисията.'

exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolCommissioner', N'IsChairman'                         , N'Председател – Да/Не.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolCommissioner', N'OrderNum'                           , N'Пореден номер.'
