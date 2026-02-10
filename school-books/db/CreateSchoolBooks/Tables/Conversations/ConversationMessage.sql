PRINT 'Create ConversationMessage table'
GO

EXEC [school_books].[spCreateIdSequence] N'ConversationMessage';
GO

CREATE TABLE [school_books].[ConversationMessage] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [ConversationId]            INT             NOT NULL,
    [ConversationMessageId]     INT             NOT NULL,
    [Content]                   NVARCHAR(MAX)   NOT NULL,
    [CreatedByParticipantId]    INT             NOT NULL,
    [CreateDate]                DATETIME2       NOT NULL,

    CONSTRAINT [PK_ConversationMessage] PRIMARY KEY ([SchoolYear], [ConversationId], [ConversationMessageId]),
    CONSTRAINT [FK_ConversationMessage_Conversation] FOREIGN KEY ([SchoolYear], [ConversationId])
        REFERENCES [school_books].[Conversation] ([SchoolYear], [ConversationId])
);
GO

CREATE NONCLUSTERED INDEX [IX_ConversationMessage_Conversation_CreateDateDesc]
ON [school_books].[ConversationMessage] ([SchoolYear], [ConversationId], [CreateDate] DESC);
GO

exec school_books.spDescTable  N'ConversationMessage', N'Съобщение за разговор.'

exec school_books.spDescColumn N'ConversationMessage', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ConversationMessage', N'ConversationMessageId'     , N'Уникален системно генериран идентификатор.'
exec school_books.spDescColumn N'ConversationMessage', N'ConversationId'            , N'Идентификатор на разговор.'
exec school_books.spDescColumn N'ConversationMessage', N'Content'                   , N'Съдържание на съобщението.'

exec school_books.spDescColumn N'ConversationMessage', N'CreatedByParticipantId'    , N'Създадено от потребител.'
exec school_books.spDescColumn N'ConversationMessage', N'CreateDate'                , N'Дата на създаване.'
