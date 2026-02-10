PRINT 'Create QualificationAcquisitionProtocolStudent table'
GO

CREATE TABLE [school_books].[QualificationAcquisitionProtocolStudent] (
    [SchoolYear]                            SMALLINT         NOT NULL,
    [QualificationAcquisitionProtocolId]    INT              NOT NULL,
    [ClassId]                               INT              NOT NULL,
    [PersonId]                              INT              NOT NULL,

    [ExamsPassed]                           BIT              NOT NULL,
    [TheoryPoints]                          DECIMAL(5,2)     NULL,
    [PracticePoints]                        DECIMAL(5,2)     NULL,
    [AverageDecimalGrade]                   DECIMAL(3,2)     NULL,

    CONSTRAINT [PK_QualificationAcquisitionProtocolStudent] PRIMARY KEY ([SchoolYear], [QualificationAcquisitionProtocolId], [ClassId], [PersonId]),
    CONSTRAINT [FK_QualificationAcquisitionProtocolStudent_QualificationAcquisitionProtocol] FOREIGN KEY ([SchoolYear], [QualificationAcquisitionProtocolId]) REFERENCES [school_books].[QualificationAcquisitionProtocol] ([SchoolYear], [QualificationAcquisitionProtocolId]),

    -- external references
    CONSTRAINT [FK_QualificationAcquisitionProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_QualificationAcquisitionProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_QualificationAcquisitionProtocolStudent_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'QualificationAcquisitionProtocolStudent', N'Протокол за придобиване на професионална квалификация - ученик.'

exec school_books.spDescColumn N'QualificationAcquisitionProtocolStudent', N'SchoolYear'                            , N'Учебна година.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocolStudent', N'QualificationAcquisitionProtocolId'    , N'Идентификатор на протокол за придобиване на професионална квалификация.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocolStudent', N'ClassId'                               , N'Идентификатор на група/паралелка, към която принадлежи ученикът.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocolStudent', N'PersonId'                              , N'Идентификатор на ученик.'

exec school_books.spDescColumn N'QualificationAcquisitionProtocolStudent', N'ExamsPassed'                           , N'Успешно положени изпити – Да/Не.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocolStudent', N'TheoryPoints'                          , N'Точки по теория на професията.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocolStudent', N'PracticePoints'                        , N'Точки по практика на професията.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocolStudent', N'AverageDecimalGrade'                   , N'Окончателна оценка - количествена оценка с точно два знака след десетичната запетая.'
