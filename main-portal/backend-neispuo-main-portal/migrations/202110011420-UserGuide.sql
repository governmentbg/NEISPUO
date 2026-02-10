CREATE TABLE [portal].[UserGuide](
	[UserGuideID] [int] NOT NULL CONSTRAINT [PK_UserGuide] PRIMARY KEY CLUSTERED,
	[Name] [nvarchar](255) NULL,
	[Url] [nvarchar](255) NULL,
	[CategoryID] [int] NOT NULL CONSTRAINT [FK_UserGuide_Category] FOREIGN KEY([CategoryID]) REFERENCES [portal].[Category] ([CategoryID])
);