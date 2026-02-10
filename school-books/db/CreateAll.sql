SET QUOTED_IDENTIFIER ON
GO

SET NOCOUNT ON
GO

PRINT '------ Creating Database'

:r "./CreateDb."$(dbEnv)".sql"

PRINT CONCAT('------ Done at ', GETDATE())
