CREATE VIEW azure_temp.UserProfile AS
SELECT
        su.Username as Username,
        su.PersonID as PersonID,
        su.SysUserID as SysUserID,
        p2.SysRoleID as SysRoleID,
        sr.Name as SysRoleName,
        RTRIM(
                LTRIM(
                        CONCAT(
                                COALESCE("p"."firstName" + ' ', ''),
                                COALESCE("p"."middleName" + ' ', ''),
                                COALESCE("p"."lastName", '')
                        )
                )
        ) AS ThreeNames,
        1 as IsRoleFromEducationalState,
        i.InstitutionID as InstitutionID,
        i.Name as InstitutionName,
        p2.PositionID as PositionID,
        p2.Name as PositionName,
        null as MunicipalityID,
        null as MunicipalityName,
        null as RegionID,
        null as RegionName,
        null as BudgetingInstitutionID,
        null as BudgetingInstitutionName,
        -- Fetch classes lead by teacher only if the selected user has Teacher Role (with RoleSysID = 5)
        CASE
                WHEN sr.SysRoleID = 5 THEN (
                        SELECT
                                ClassID --- inst_year.ClassTeacher199 - Database View which contains information about Lead Teachers
                        FROM
                                inst_year.ClassTeacher199 leadClassTeachers
                        WHERE
                                leadClassTeachers.PersonID = p.PersonID
                                AND leadClassTeachers.InstitutionID = es.InstitutionID FOR JSON AUTO
                )
                ELSE NULL
        END AS LeadTeacherClasses
FROM
        core.SysUser su
        left join core.Person p on p.PersonID = su.PersonID
        left join core.EducationalState es on es.PersonID = p.PersonID
        left join core.[Position] p2 on p2.PositionID = es.PositionID
        left join core.Institution i on i.InstitutionID = es.InstitutionID -- inner join as we want to only select rows that would lead to a role match
        inner join core.SysRole sr on sr.SysRoleID = p2.SysRoleID
UNION
-- Select roles from SysUserSysRole table
SELECT
        su2.Username as Username,
        su2.PersonID as PersonID,
        su2.SysUserID as SysUserID,
        susr.SysRoleID as SysRoleID,
        sr2.Name as SysRoleName,
        RTRIM(
                LTRIM(
                        CONCAT(
                                COALESCE("p3"."firstName" + ' ', ''),
                                COALESCE("p3"."middleName" + ' ', ''),
                                COALESCE("p3"."lastName", '')
                        )
                )
        ) AS ThreeNames,
        0 as IsRoleFromEducationalState,
        susr.InstitutionID as InstitutionID,
        i2.Name as InstitutionName,
        null as PositionID,
        null as PositionName,
        susr.MunicipalityID as MunicipalityID,
        m.Name as MunicipalityName,
        susr.RegionID as RegionID,
        r.Name as RegionName,
        susr.BudgetingInstitutionID as BudgetingInstitutionID,
        bi.Name as BudgetingInstitutionName,
        null as LeadTeacherClasses
FROM
        core.SysUser su2 -- inner join as we want to only select rows that would lead to a role match
        inner join core.SysUserSysRole susr on susr.SysUserID = su2.SysUserID
        left join core.SysRole sr2 on sr2.SysRoleID = susr.SysRoleID
        left join core.Person p3 on p3.PersonID = su2.PersonID
        left join location.Region r on r.RegionID = susr.RegionID
        left join location.Municipality m on m.MunicipalityID = susr.MunicipalityID
        left join core.Institution i2 on i2.InstitutionID = susr.InstitutionID
        left join noms.BudgetingInstitution bi on bi.BudgetingInstitutionID = susr.BudgetingInstitutionID