GO

SET IDENTITY_INSERT core.Person ON
INSERT INTO core.Person(PersonID, FirstName, LastName)
VALUES (3, N'Модул Дневници', N'Системен')
SET IDENTITY_INSERT core.Person OFF
GO

SET IDENTITY_INSERT core.SysUser ON
INSERT INTO core.SysUser(SysUserID, Username, IsAzureUser, PersonID, isAzureSynced)
VALUES (3, 'schoolbooks', 0, 3, 0)
SET IDENTITY_INSERT core.SysUser OFF
GO

UPDATE school_books.Absence
SET ModifiedBySysUserId = 3
WHERE ModifiedBySysUserId = 1154605;
GO
