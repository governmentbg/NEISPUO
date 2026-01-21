PRINT 'Create fn_join_names3 function'
GO

CREATE OR ALTER FUNCTION [school_books].fn_join_names3 (@firstName NVARCHAR(MAX), @middleName NVARCHAR(MAX), @lastName NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN RTRIM(LTRIM(
        CONCAT(
            COALESCE(@firstName + ' ', '')
            , COALESCE(@middleName + ' ', '')
            , COALESCE(@lastName, '')
        )
    ));
END
GO

PRINT 'Create fn_join_names2 function'
GO

CREATE OR ALTER FUNCTION [school_books].fn_join_names2 (@firstName NVARCHAR(MAX), @lastName NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN school_books.fn_join_names3(@firstName, NULL, @lastName);
END
GO