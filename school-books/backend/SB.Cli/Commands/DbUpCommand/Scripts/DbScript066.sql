GO

CREATE INDEX [IX_SchoolBooksLog_DateUtc] ON [logs].[SchoolBooksLog] ([App], [DateUtc]) INCLUDE ([RequestId])
GO
