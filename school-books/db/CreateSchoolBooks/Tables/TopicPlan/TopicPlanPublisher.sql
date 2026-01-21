PRINT 'Create TopicPlanPublisher table'
GO

CREATE TABLE [school_books].[TopicPlanPublisher] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_TopicPlanPublisher] PRIMARY KEY ([Id])
);
GO

exec school_books.spDescTable  N'TopicPlanPublisher', N'Издателство.'

exec school_books.spDescColumn N'TopicPlanPublisher', N'Id'         , N'Уникален идентификатор.'

exec school_books.spDescColumn N'TopicPlanPublisher', N'Name'       , N'Наименование.'
exec school_books.spDescColumn N'TopicPlanPublisher', N'Description', N'Описание.'
exec school_books.spDescColumn N'TopicPlanPublisher', N'IsValid'    , N'Валидност – да/не.'
exec school_books.spDescColumn N'TopicPlanPublisher', N'ValidFrom'  , N'Валидност от.'
exec school_books.spDescColumn N'TopicPlanPublisher', N'ValidTo'    , N'Валидност до.'
exec school_books.spDescColumn N'TopicPlanPublisher', N'SortOrd'    , N'Номер на подредба.'
