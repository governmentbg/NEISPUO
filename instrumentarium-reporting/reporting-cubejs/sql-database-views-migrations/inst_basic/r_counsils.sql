CREATE VIEW [inst_basic].[R_councils] AS

select 
	i.Name as 'InstitutionName',
	ipc.FirstName as 'FirstName',
	ipc.MiddleName as 'MiddleName',
	ipc.FamilyName as 'LastName',
	ipc.PhoneNumber as 'Phone',
	ipc.Email as 'Email',
	pcr.Name as 'Role',
	pct.Name as 'CouncilType'

from core.Institution i
join inst_basic.InstitutionPublicCouncil ipc on i.InstitutionID = ipc.InstitutionID
join inst_nom.PublicCouncilRole pcr on pcr.PublicCouncilRoleID = ipc.PublicCouncilRoleID
join inst_nom.PublicCouncilType pct on pct.PublicCouncilTypeID = ipc.PublicCouncilTypeID
where
pcr.IsValid = 1
GO

