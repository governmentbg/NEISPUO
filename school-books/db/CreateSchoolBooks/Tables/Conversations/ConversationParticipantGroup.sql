PRINT 'Create ConversationParticipantGroup table'
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

exec school_books.spDescTable  N'ConversationParticipantGroup', N'Група на участници в разговор.'

exec school_books.spDescColumn N'ConversationParticipantGroup', N'SchoolYear'                        , N'Учебна година.'
exec school_books.spDescColumn N'ConversationParticipantGroup', N'ConversationParticipantGroupId'    , N'Идентификатор на групата.'
exec school_books.spDescColumn N'ConversationParticipantGroup', N'GroupName'                         , N'Наименование на групата.'
exec school_books.spDescColumn N'ConversationParticipantGroup', N'ParticipantType'                   , N'Тип на участника - всички учители, всички родители, всички ученици.'
exec school_books.spDescColumn N'ConversationParticipantGroup', N'ClassBookId'                       , N'Номер на клас.'
