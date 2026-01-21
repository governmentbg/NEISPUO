PRINT 'Create fn_check_unique_person function'
GO

CREATE OR ALTER FUNCTION [core].[fn_check_unique_person](
	@FirstName nvarchar(255)
	, @MiddleName nvarchar(255)
	, @LastName nvarchar(255)
	, @BirthDate date
	, @PersonalIdType int
	, @SysUserType int
	, @Date datetime
)
RETURNS INT
AS
BEGIN
	IF @Date < DATEADD(day, -1, GetDate())
	 OR
		@SysUserType != 2 -- Ученик
			OR @PersonalIdType != 2 -- ИДН
				OR @BirthDate IS NULL
		BEGIN
			RETURN 0;
		END
	ELSE
		IF EXISTS (SELECT 1 FROM core.Person
			WHERE MiddleName IS NOT NULL AND BirthDate IS NOT NULL AND PersonalIDType IS NOT NULL
				AND FirstName = @FirstName AND MiddleName = @MiddleName AND LastName = @LastName AND BirthDate = @BirthDate AND PersonalIDType <> 2)
		BEGIN
			RETURN 1;
		END

	RETURN 0;

END
GO
