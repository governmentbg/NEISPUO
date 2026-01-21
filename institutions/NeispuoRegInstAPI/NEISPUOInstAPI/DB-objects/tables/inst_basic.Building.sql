USE [neispuo]
GO

/****** Object:  Table [inst_basic].[Building]    Script Date: 10.10.2021 г. 16:25:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [inst_basic].[Building](
	[BuildingID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NULL,
	[InstitutionDepartmentID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[BuildingTypeID] [int] NULL,
	[ExpectedStudentCount] [int] NULL,
	[StructureCount] [int] NULL,
	[SchoolShiftTypeID] [int] NULL,
	[CadastralCode] [nvarchar](50) NULL,
	[InitialOwnershipDoc] [int] NULL,
	[LatestOwnershipDoc] [int] NULL,
	[LatestRepairYear] [int] NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[Note] [nvarchar](4000) NULL,
 CONSTRAINT [PK_Building] PRIMARY KEY CLUSTERED 
(
	[BuildingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

