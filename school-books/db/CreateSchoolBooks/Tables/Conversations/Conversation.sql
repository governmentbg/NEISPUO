PRINT 'Create Conversation table'
GO

EXEC [school_books].[spCreateIdSequence] N'Conversation';
GO

CREATE TABLE [school_books].[Conversation] (
    [SchoolYear]                SMALLINT             NOT NULL,
    [ConversationId]            INT                  NOT NULL,
    [ParticipantsInfo]          NVARCHAR(MAX)        NULL,
    [Title]                     NVARCHAR(1000)       NOT NULL,
    [CreateDate]                DATETIME2            NOT NULL,
    [LastMessageId]             INT                  NULL,
    [LastMessageDate]           DATETIME2            NOT NULL,
    [IsLocked]                  BIT                  NOT NULL,
    [Version]                   ROWVERSION           NOT NULL,

    CONSTRAINT [PK_Conversation] PRIMARY KEY ([SchoolYear], [ConversationId])
);
GO

exec school_books.spDescTable  N'Conversation', N'Разговор.'

exec school_books.spDescColumn N'Conversation', N'SchoolYear'               , N'Учебна година.'
exec school_books.spDescColumn N'Conversation', N'ConversationId'           , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Conversation', N'ParticipantsInfo'         , N'Информация за участниците.'
exec school_books.spDescColumn N'Conversation', N'Title'                    , N'Заглавие на разговора.'
exec school_books.spDescColumn N'Conversation', N'CreateDate'               , N'Дата на създаване.'
exec school_books.spDescColumn N'Conversation', N'LastMessageId'            , N'Дата на последно добавено съобщение.'
exec school_books.spDescColumn N'Conversation', N'IsLocked'                 , N'Заключен разговор.'

exec school_books.spDescColumn N'Conversation', N'Version'                   , N'Версия.'
