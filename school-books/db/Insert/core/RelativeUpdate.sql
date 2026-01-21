GO
PRINT 'Update [family].[Relative]'
GO

DECLARE @minPersonOrder INT = 0;
DECLARE @maxPersonOrder INT = 3200;

UPDATE r
SET
    [FirstName] = pn.[FirstName],
    [MiddleName] = pn.[MiddleName],
    [LastName] = pn.[LastName],
    [PersonalID] = pn.[EGN],
    [PersonalIDType] = 0,
    [Notes] = 'тест бележки',
    [Description] = 'тест описание',
    [Email] = 'test@example.com',
    [PhoneNumber] = '0888111222'
FROM [family].[Relative] r
JOIN [#tempPersonNames] pn ON FLOOR(RAND(r.[RelativeID])*(@minPersonOrder-@maxPersonOrder+1))+@maxPersonOrder = pn.[Order]

GO
