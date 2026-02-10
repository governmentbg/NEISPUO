ALTER TABLE [school_books].[StudentSettings]
ADD
    [AllowMessageEmails] BIT NOT NULL DEFAULT 0,
    [AllowMessageNotifications] BIT NOT NULL DEFAULT 0
GO
