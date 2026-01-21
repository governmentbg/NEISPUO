PRINT 'Create vwExtInstitutionStaff'
GO

CREATE OR ALTER VIEW [school_books].[vwExtInstitutionStaff]
AS
    SELECT
        p.PersonID,
        p.PublicEduNumber,
        p.FirstName,
        p.MiddleName,
        p.LastName,
        su.SysUserID,
        su.Username,
        sp.InstitutionID,
        sp.StaffPositionID,
        sp.StaffTypeID,
        st.Name AS StaffTypeName,
        sp.CategoryStaffTypeID,
        cst.Name AS CategoryStaffTypeName,
        sp.NKPDPositionID,
        pos.Name AS NKPDPositionName,
        sp.PositionKindID,
        pk.Name AS PositionKindName
    FROM
        [core].[Person] p
        LEFT JOIN [core].[SysUser] su on p.PersonID = su.PersonID
        INNER JOIN [inst_basic].[StaffPosition] sp on p.PersonID = sp.PersonID
        INNER JOIN [inst_nom].[StaffType] st on sp.StaffTypeID = st.StaffTypeID
        INNER JOIN [inst_nom].[CategoryStaffType] cst on sp.CategoryStaffTypeID = cst.CategoryStaffTypeID
        INNER JOIN [inst_nom].[NKPDPosition] pos on sp.NKPDPositionID = pos.NKPDPositionID
        INNER JOIN [inst_nom].[PositionKind] pk on sp.PositionKindID = pk.PositionKindID
GO
