PRINT 'Create StudentSettings table'
GO

EXEC [school_books].[spCreateIdSequence] N'StudentSettings'
GO

CREATE TABLE [school_books].[StudentSettings] (
    [StudentSettingsId]         INT              NOT NULL,
    [PersonId]                  INT              NOT NULL,
    [AllowGradeEmails]          BIT              NOT NULL,
    [AllowAbsenceEmails]        BIT              NOT NULL,
    [AllowRemarkEmails]         BIT              NOT NULL,
    [AllowMessageEmails]        BIT              NOT NULL,
    [AllowGradeNotifications]   BIT              NOT NULL,
    [AllowAbsenceNotifications] BIT              NOT NULL,
    [AllowRemarkNotifications]  BIT              NOT NULL,
    [AllowMessageNotifications] BIT              NOT NULL,

    [Version]                   ROWVERSION       NOT NULL,

    CONSTRAINT [PK_StudentSettings] PRIMARY KEY ([StudentSettingsId]),

    -- external references
    CONSTRAINT [FK_StudentSettings_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),

    INDEX [IX__StudentSettingsPersonId] UNIQUE ([PersonId]),
);
GO

exec school_books.spDescTable  N'StudentSettings', N'Настройки за потребители с роля родители/ученици.'

exec school_books.spDescColumn N'StudentSettings', N'StudentSettingsId'             , N'Идентификатор на настройки за потребители с роля родители/ученици.'
exec school_books.spDescColumn N'StudentSettings', N'PersonId'                      , N'Идентификатор на потребителя.'
exec school_books.spDescColumn N'StudentSettings', N'AllowGradeEmails'              , N'Изпращане на имейли за оценки – да/не.'
exec school_books.spDescColumn N'StudentSettings', N'AllowAbsenceEmails'            , N'Изпращане на имейли за отсъствия – да/не.'
exec school_books.spDescColumn N'StudentSettings', N'AllowRemarkEmails'             , N'Изпращане на имейли за отзиви – да/не.'
exec school_books.spDescColumn N'StudentSettings', N'AllowMessageEmails'           , N'Изпращане на имейли за съобщения – да/не.'
exec school_books.spDescColumn N'StudentSettings', N'AllowGradeNotifications'       , N'Изпращане на известия за оценки – да/не.'
exec school_books.spDescColumn N'StudentSettings', N'AllowAbsenceNotifications'     , N'Изпращане на известия за отсъствия – да/не.'
exec school_books.spDescColumn N'StudentSettings', N'AllowRemarkNotifications'      , N'Изпращане на известия за отзиви – да/не.'
exec school_books.spDescColumn N'StudentSettings', N'AllowMessageNotifications'    , N'Изпращане на известия за съобщения – да/не.'

exec school_books.spDescColumn N'StudentSettings', N'Version'                       , N'Версия.'
