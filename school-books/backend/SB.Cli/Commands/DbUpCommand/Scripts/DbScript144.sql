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

EXEC [school_books].[spCreateIdSequence] N'ConversationParticipantGroup';
GO

CREATE TABLE [school_books].[ConversationParticipantGroup] (
    [SchoolYear]                        SMALLINT           NOT NULL,
    [ConversationParticipantGroupId]    INT                NOT NULL,
    [GroupName]                         NVARCHAR(255)      NOT NULL,
    [ParticipantType]                   SMALLINT           NOT NULL,
    [ClassBookId]                       INT                NULL,

    CONSTRAINT [PK_ConversationParticipantGroup] PRIMARY KEY ([SchoolYear], [ConversationParticipantGroupId]),

    CONSTRAINT [CHK_Conversation_Group_Type] CHECK ([ParticipantType] IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10)),
);
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
    [LastReadMessageDate]               DATETIME2           NULL,
    [LastReadMessageId]                 INT                 NULL,
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
