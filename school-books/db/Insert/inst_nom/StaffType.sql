GO
PRINT 'Insert [inst_nom].[StaffType]'
GO

insert [inst_nom].[StaffType] ([StaffTypeID],[Name],[Description],[IsValid],[ValidFrom],[ValidTo])
select 1,'педагогически',NULL,1,'2020-12-02 00:00:00.000',NULL UNION ALL
select 2,'непедагогически',NULL,1,'2020-12-02 00:00:00.000',NULL;

GO
