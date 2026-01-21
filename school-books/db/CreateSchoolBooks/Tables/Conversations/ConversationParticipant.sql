PRINT 'Create ConversationParticipant table'
GO

EXEC [school_books].[spCreateIdSequence] N'ConversationParticipant';
GO


CREATE TABLE [school_books].[ConversationParticipant] (
    [SchoolYear]                        SMALLINT            NOT NULL,
    [InstId]                            INT                 NOT NULL,
    [ConversationId]                    INT                 NOT NULL,
    [ConversationParticipantId]         INT                 NOT NULL,
    [SysUserId]                         INT                 NOT NULL,
    [Title]                             NVARCHAR(255)       NOT NULL,
    [ParticipantType]                   SMALLINT            NOT NULL,
    [ConversationParticipantGroupId]    INT                 NULL,
    [LastReadMessageId]                 INT                 NULL,
    [LastReadMessageDate]               DATETIME2           NULL,
	[IsCreator]                         BIT                 NOT NULL,

    CONSTRAINT [PK_ConversationParticipant] PRIMARY KEY ([SchoolYear], [ConversationId], [ConversationParticipantId]),
    CONSTRAINT [FK_ConversationParticipant_Conversation] FOREIGN KEY ([SchoolYear], [ConversationId])
        REFERENCES [school_books].[Conversation] ([SchoolYear], [ConversationId]),
    CONSTRAINT [FK_ConversationParticipant_SysUser] FOREIGN KEY ([SysUserId])
        REFERENCES [core].[SysUser] ([SysUserId]),
    CONSTRAINT [FK_ConversationParticipant_Group] FOREIGN KEY ([SchoolYear], [ConversationParticipantGroupId])
        REFERENCES [school_books].[ConversationParticipantGroup] ([SchoolYear], [ConversationParticipantGroupId]),

    CONSTRAINT [CHK_Conversation_Participant_Type] CHECK ([ParticipantType] IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10)),
);
GO

CREATE NONCLUSTERED INDEX [IX_ConversationParticipant_SysUser_LastReadMessageDate]
ON [school_books].[ConversationParticipant] ([SchoolYear], [SysUserId], [LastReadMessageDate]);
GO

exec school_books.spDescTable  N'ConversationParticipant', N'Участник в разговор.'

exec school_books.spDescColumn N'ConversationParticipant', N'SchoolYear'                        , N'Учебна година.'
exec school_books.spDescColumn N'ConversationParticipant', N'InstId'                            , N'Номер на институция.'
exec school_books.spDescColumn N'ConversationParticipant', N'ConversationId'                    , N'Идентификатор на разговор.'

exec school_books.spDescColumn N'ConversationParticipant', N'ConversationParticipantId'         , N'Идентификатор на участника.'
exec school_books.spDescColumn N'ConversationParticipant', N'SysUserId'                         , N'Референция към потребители.'
exec school_books.spDescColumn N'ConversationParticipant', N'Title'                             , N'Наименование на участника.'
exec school_books.spDescColumn N'ConversationParticipant', N'ParticipantType'                   , N'Тип на участника - учител, родител, ученик.'
exec school_books.spDescColumn N'ConversationParticipant', N'ConversationParticipantGroupId'    , N'Идентификатор за група.'
exec school_books.spDescColumn N'ConversationParticipant', N'LastReadMessageId'                 , N'Последно прочетено съобщение.'
exec school_books.spDescColumn N'ConversationParticipant', N'LastReadMessageDate'               , N'Дата на последно прочетено съобщение.'
exec school_books.spDescColumn N'ConversationParticipant', N'IsCreator'                         , N'Създател ли е на разговора.'
