PRINT 'Create fn_closest_weekday function'
GO

CREATE OR ALTER FUNCTION [school_books].fn_closest_weekday (@date DATE)
RETURNS DATE
AS
BEGIN
    DECLARE @weekday INT;
    SELECT @weekday = ((DATEPART(WEEKDAY, @date) + @@DATEFIRST + 5) % 7) + 1

    DECLARE @offset INT;
    SELECT @offset =
      CASE @weekday
         WHEN 6 THEN 2
         WHEN 7 THEN 1
         ELSE 0
      END

    RETURN DATEADD(DAY, @offset, @date);
END
GO
