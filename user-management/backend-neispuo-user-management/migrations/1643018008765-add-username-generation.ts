import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { IsAzureUser } from 'src/common/constants/enum/is-azure-user.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddUsernameGeneration1643018008765 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `	
            CREATE PROCEDURE  azure_temp.GENERATE_USERNAME(
                @FirstName VARCHAR(100),
                @MiddleName VARCHAR(100),
                @LastName VARCHAR(100),
                @Email VARCHAR(100),
                @AzureUserRole VARCHAR(100),
                @PersonID INT,
                @ERRORS VARCHAR(100) OUTPUT,
                @RESULT VARCHAR(100) OUTPUT
            ) AS
            BEGIN
                BEGIN TRY
                    SET NOCOUNT ON
                    DECLARE @FirstNameInitial VARCHAR(1);
                    DECLARE @MiddleNameInitial VARCHAR(1);
                    DECLARE @LastNameInitial VARCHAR(1);
                    DECLARE @PartialMiddleName VARCHAR(100);
                    DECLARE @AzureStudentUsername VARCHAR(100);
                    DECLARE @AzureTeacherUsername VARCHAR(100);
                    DECLARE @AzureParentUsername VARCHAR(100);
                    DECLARE @MiddleNameLength INT;
                    DECLARE @AzureTeacherHasThreeNames INT;
                    DECLARE @PersonIDExists INT;
                    DECLARE @AzureStudentUsernameExists INT;
                    DECLARE @AzureTeacherUsernameExists INT;
                    DECLARE @AzureTeacherUsernameIsInserted INT = 0;
                    DECLARE @AzureTeacherUsernameOccurrences INT;
                    DECLARE @AzureParentUsernameOccurrences INT;
                    DECLARE @ParamsAreInvalid INT = 0;
                    DECLARE @COUNTER INT = 0;
                    DECLARE @MAX_RETRY_ATTEMPTS INT = 10;
                    DECLARE @AzureStudentNumber INT;
                    RAISERROR ('FirstName is: %s', 0, 1, @FirstName) WITH NOWAIT;
                    IF @AzureUserRole = 'parent' AND @Email IS NULL
                        BEGIN
                            SET @ParamsAreInvalid = 1;
                            SET @ERRORS = 'INVALID PARAM: @Email CANNOT BE NULL.';
                            RETURN 1;
                        END
                    IF @FirstName IS NULL
                        BEGIN
                            SET @ParamsAreInvalid = 1;
                            SET @ERRORS = 'INVALID PARAM: @FirstName CANNOT BE NULL.';
                            RETURN 1;
                        END
                    RAISERROR ('LastName is: %s', 0, 1, @LastName) WITH NOWAIT;
                    IF @LastName IS NULL
                        BEGIN
                            SET @ParamsAreInvalid = 1;
                            SET @ERRORS = 'INVALID PARAM: @LastName CANNOT BE NULL.';
                            RETURN 1;
                        END
                    RAISERROR ('AzureUserRole is: %s', 0, 1, @AzureUserRole) WITH NOWAIT;
                    IF @AzureUserRole IS NULL
                        BEGIN
                            SET @ParamsAreInvalid = 1;
                            SET @ERRORS = 'INVALID PARAM: @AzureUserRole CANNOT BE NULL.';
                            RETURN 1;
                        END
                    IF @PersonID IS NULL
                        BEGIN
                            SET @ParamsAreInvalid = 1;
                            SET @ERRORS = 'INVALID PARAM: @PersonID CANNOT BE NULL.';
                            RETURN 1;
                        END
                    SET @FirstName = cast(@FirstName as varchar(100));
                    SET @MiddleName = cast(@MiddleName as varchar(100));
                    SET @LastName = cast(@LastName as varchar(100));
                    SET @Email = cast(@Email as varchar(100));
                    SET @AzureUserRole = cast(@AzureUserRole as varchar(100));
                    RAISERROR ('cast(FirstName) as varchar is: %s', 0, 1, @FirstName) WITH NOWAIT;
                    RAISERROR ('cast(MiddleName) as varchar is: %s', 0, 1, @MiddleName) WITH NOWAIT;
                    RAISERROR ('cast(LastName) as varchar is: %s', 0, 1, @LastName) WITH NOWAIT;
                    RAISERROR ('cast(Email) as varchar is: %s', 0, 1, @Email) WITH NOWAIT;
                    RAISERROR ('cast(AzureUserRole) as varchar is: %s', 0, 1, @AzureUserRole) WITH NOWAIT;
                    IF (LEN(@FirstName) <= 0 OR LEN(@FirstName) > 100)
                        BEGIN
                            SET @ParamsAreInvalid = 1;
                            SET @ERRORS = 'INVALID PARAM: @FirstName. MUST BETWEEN 1-100 CHARS';
                            RETURN 1;
                        END
                    IF (LEN(@MiddleName) > 100)
                        BEGIN
                            SET @ParamsAreInvalid = 1;
                            SET @ERRORS = 'INVALID PARAM: @MiddleName. MUST BETWEEN 0-100 CHARS ';
                            RETURN 1;
                        END
                    IF (LEN(@LastName) <= 0 OR LEN(@LastName) > 100)
                        BEGIN
                            SET @ParamsAreInvalid = 1;
                            SET @ERRORS = 'INVALID PARAM: @LastName. MUST BETWEEN 1-100 CHARS';
                            RETURN 1;
                        END
                    IF (@AzureUserRole = 'parent' AND (LEN(@Email) <= 0 OR LEN(@Email) > 100))
                        BEGIN
                            SET @ParamsAreInvalid = 1;
                            SET @ERRORS = 'INVALID PARAM: @Email. MUST BETWEEN 1-100 CHARS';
                            RETURN 1;
                        END
                    RAISERROR ('AzureUserRole is: %s', 0, 1, @AzureUserRole) WITH NOWAIT;
                    IF (@AzureUserRole NOT LIKE 'student' AND @AzureUserRole NOT LIKE 'teacher' AND @AzureUserRole NOT LIKE 'parent')
                        BEGIN
                            SET @ParamsAreInvalid = 1;
                            SET @ERRORS = 'INVALID PARAM: @AzureUserRole. MUST BE STUDENT TEACHER OR PARENT';
                            RETURN 1;
                        END
                    
                    SELECT @PersonIDExists = COUNT(1) FROM core.SysUser su WHERE su.PersonID = @PersonID AND su.DeletedOn IS NULL;
                    IF @PersonIDExists = 1
                        BEGIN
                            SET @ParamsAreInvalid = 1;
                            SET @ERRORS = 'INVALID PARAM: @PersonID already exists in core.SysUser table';
                            RETURN 1;
                        END
                    IF @AzureUserRole = 'student'
                        BEGIN
                                RAISERROR ('User is student', 0, 1) WITH NOWAIT;
                                SET @FirstNameInitial = SUBSTRING(@FirstName, 1, 1);
                                RAISERROR ('FirstNameInitial is: %s', 0, 1, @FirstNameInitial) WITH NOWAIT;
                                SET @LastNameInitial = SUBSTRING(@MiddleName, 1, 1);
                                RAISERROR ('LastNameInitial is: %s', 0, 1, @LastNameInitial) WITH NOWAIT;
                                RAISERROR ('AzureStudentUsername is: %s', 0, 1, @AzureStudentUsername) WITH NOWAIT;
                                SET @AzureStudentUsernameExists = 1
                                RAISERROR ('AzureStudentUsernameExists is: %d', 0, 1, @AzureStudentUsernameExists) WITH NOWAIT;
                                WHILE @AzureStudentUsernameExists = 1 AND @COUNTER < @MAX_RETRY_ATTEMPTS
                                    BEGIN
                                        SELECT @AzureStudentNumber = LEFT(CAST(RAND()*1000000000+99999999 AS INT),8);
                                        RAISERROR ('AzureStudentNumber is: %d', 0, 1, @AzureStudentNumber) WITH NOWAIT;
                                        SET @AzureStudentUsername = CONCAT(CONCAT(@FirstNameInitial, @LastNameInitial), CAST(@AzureStudentNumber AS varchar));
                                        RAISERROR ('AzureStudentUsername is: %s', 0, 1, @AzureStudentUsername) WITH NOWAIT;
                                        IF NOT EXISTS (SELECT TOP 1 1 FROM azure_temp.GeneratedUsers gu WHERE gu.Username = @AzureStudentUsername)
                                            BEGIN
                                                SET @AzureStudentUsernameExists = 0;
                                                BREAK;
                                            END
                                        SET @COUNTER = @COUNTER + 1;
                                    END
                                IF @AzureStudentUsernameExists = 0 
                                    BEGIN
                                        RAISERROR ('AzureStudentUsernameExists is: %d', 0, 1, @AzureStudentUsernameExists) WITH NOWAIT;
                                        RAISERROR ('INSERTING Username', 0, 1) WITH NOWAIT;
                                        INSERT INTO
                                            azure_temp.GeneratedUsers(
                                                Username
                                            )
                                        VALUES (
                                            @AzureStudentUsername
                                        );
                                        RAISERROR ('INSERTED Username', 0, 1) WITH NOWAIT;
                                    END
                                ELSE
                                    BEGIN
                                        SET @ERRORS = 'ERROR: ERROR WITH STUDENT NAME GENERATION OR REACHED MAX RETRY ATTEMPTS.';
                                        RETURN 1;
                                    END
                                
                            SET @RESULT = @AzureStudentUsername;
                            RAISERROR ('RESULT is: %s', 0, 1, @RESULT) WITH NOWAIT;
                        END
                    IF @AzureUserRole = 'teacher'
                        BEGIN
                            RAISERROR ('User is teacher', 0, 1) WITH NOWAIT;
                            SELECT @AzureTeacherHasThreeNames = CASE WHEN COALESCE(@MiddleName, '') = '' THEN 0 ELSE 1 END;
                            IF @AzureTeacherHasThreeNames = 0
                                BEGIN
                                    RAISERROR ('AzureTeacherHasThreeNames is: %d', 0, 1, @AzureTeacherHasThreeNames) WITH NOWAIT;
                                    SET @MiddleNameLength = 0;
                                    SET @AzureTeacherUsername = CONCAT(@FirstName, '.' , @LastName);
                                END
                            ELSE
                                BEGIN
                                    RAISERROR ('AzureTeacherHasThreeNames is: %d', 0, 1, @AzureTeacherHasThreeNames) WITH NOWAIT;
                                    SELECT @MiddleNameLength = LEN(@MiddleName);
                                    SET @AzureTeacherUsername = CONCAT(@FirstName, '.' , @MiddleName, '.' , @LastName);
                                END
                        --------THIS IS PARTIAL MIDDLENAME	
                            SET @COUNTER = 0;
                            RAISERROR ('AzureTeacherUsername is: %s', 0, 1, @AzureTeacherUsername) WITH NOWAIT;
                            SELECT @AzureTeacherUsernameOccurrences = COALESCE(gu.Occurrences,0) FROM azure_temp.GeneratedUsers gu WHERE gu.Username = @AzureTeacherUsername;  		
                            RAISERROR ('AzureTeacherUsernameOccurrences is: %d', 0, 1, @AzureTeacherUsernameOccurrences) WITH NOWAIT;
                            IF @AzureTeacherUsernameOccurrences > 0
                                BEGIN
                                    --this will skipp the middleName loop if the fullname already exists.
                                    SET @COUNTER = @MiddleNameLength + 1;
                                END
                            RAISERROR ('COUNTER is: %d', 0, 1, @COUNTER) WITH NOWAIT;
                            RAISERROR ('MiddleNameLength is: %d', 0, 1, @MiddleNameLength) WITH NOWAIT;
                            WHILE @COUNTER <= @MiddleNameLength
                                BEGIN
                                    RAISERROR ('Inside middle name loop', 0, 1) WITH NOWAIT;
                                    RAISERROR ('MiddleNameLength is: %d', 0, 1, @MiddleNameLength) WITH NOWAIT;  
                            
                                    SET @PartialMiddleName = SUBSTRING(@MiddleName, 1, @COUNTER);
                                    RAISERROR ('PartialMiddleName is: %s', 0, 1, @PartialMiddleName) WITH NOWAIT;
                                    IF @COUNTER = 0
                                        BEGIN
                                            SET @AzureTeacherUsername = CONCAT(@FirstName, '.', @LastName);
                                        END
                                    ELSE
                                        BEGIN
                                            SET @AzureTeacherUsername = CONCAT(@FirstName, '.' , @PartialMiddleName, '.' , @LastName);
                                        END
                                    RAISERROR ('AzureTeacherUsername is: %s', 0, 1, @AzureTeacherUsername) WITH NOWAIT;
                                    SELECT @AzureTeacherUsernameExists = COUNT(1) FROM azure_temp.GeneratedUsers gu WHERE gu.Username = @AzureTeacherUsername    		
                                    
                                    RAISERROR ('AzureTeacherUsernameExists is: %d', 0, 1, @AzureTeacherUsernameExists) WITH NOWAIT;
                                    IF @AzureTeacherUsernameExists = 0
                                        BEGIN
                                            RAISERROR ('Inside AzureTeacherUsernameExists if', 0, 1) WITH NOWAIT;
                                            RAISERROR ('INSERTING Username', 0, 1) WITH NOWAIT;
                                            INSERT INTO
                                                azure_temp.GeneratedUsers(
                                                    Username
                                                )
                                            VALUES (
                                                @AzureTeacherUsername
                                            );
                                            RAISERROR ('INSERTED Username', 0, 1) WITH NOWAIT;
                                            SET @AzureTeacherUsernameIsInserted = 1;
                                            RAISERROR ('AzureTeacherUsernameIsInserted is: %d', 0, 1, @AzureTeacherUsernameIsInserted) WITH NOWAIT;
                                            BREAK;
                                        END
                                    SET @COUNTER = @COUNTER + 1;
                                END		
                            IF @AzureTeacherUsernameIsInserted = 0
                                BEGIN
                                    RAISERROR ('AzureTeacherUsername is: %s', 0, 1, @AzureTeacherUsername) WITH NOWAIT;
                                    SELECT @AzureTeacherUsernameOccurrences = COALESCE(gu.Occurrences,0) FROM azure_temp.GeneratedUsers gu WHERE gu.Username = @AzureTeacherUsername;  		
                                    RAISERROR ('AzureTeacherUsernameOccurrences is: %d', 0, 1, @AzureTeacherUsernameOccurrences) WITH NOWAIT;
                                    IF @AzureTeacherUsernameOccurrences > 0
                                        BEGIN
                                            RAISERROR ('Inside AzureTeacherUsernameOccurrences', 0, 1) WITH NOWAIT;
                                            RAISERROR ('UPDATE Username', 0, 1) WITH NOWAIT;
                                            UPDATE
                                                azure_temp.GeneratedUsers
                                            SET
                                                Occurrences = Occurrences + 1
                                            WHERE Username = @AzureTeacherUsername;
                                            RAISERROR ('UPDATED Username', 0, 1) WITH NOWAIT;
                                            RAISERROR ('INSERTING Username', 0, 1) WITH NOWAIT;
                                            RAISERROR ('INSERTING Username: %s', 0, 1, @AzureTeacherUsernameOccurrences) WITH NOWAIT;
                                            SET @AzureTeacherUsername = CONCAT(@AzureTeacherUsername, CAST(@AzureTeacherUsernameOccurrences AS varchar));
                                            INSERT INTO
                                                azure_temp.GeneratedUsers(
                                                    Username
                                                )
                                            VALUES (
                                                @AzureTeacherUsername
                                            );
                                            RAISERROR ('INSERTED Username', 0, 1) WITH NOWAIT;
                                        END
                                    ELSE
                                        BEGIN
                                            RAISERROR ('INSERTING Username', 0, 1) WITH NOWAIT;
                                            INSERT INTO
                                                azure_temp.GeneratedUsers(
                                                    Username
                                                )
                                            VALUES (
                                                @AzureTeacherUsername
                                            );
                                            RAISERROR ('INSERTED Username', 0, 1) WITH NOWAIT;
                                        END
                                END
                            SET @RESULT = @AzureTeacherUsername;
                            RAISERROR ('RESULT: %s', 0, 1, @RESULT) WITH NOWAIT;
                        END
                    IF @AzureUserRole = 'parent'
                        BEGIN
                            SET @AzureParentUsername = @Email;
                            RAISERROR ('AzureTeacherUsername is: %s', 0, 1, @AzureTeacherUsername) WITH NOWAIT;
                            SELECT @AzureParentUsernameOccurrences = COALESCE(gu.Occurrences,0) FROM azure_temp.GeneratedUsers gu WHERE gu.Username = @AzureParentUsername;  		
                            RAISERROR ('AzureTeacherUsernameOccurrences is: %d', 0, 1, @AzureParentUsernameOccurrences) WITH NOWAIT;
                            IF @AzureParentUsernameOccurrences > 0
                                BEGIN
                                    RAISERROR ('Inside AzureParentUsernameOccurrences', 0, 1) WITH NOWAIT;
                                    SET @ERRORS = 'ERROR: ERROR WITH PARENT NAME GENERATION OR PARENT ALREADY EXISTS';
                                    RETURN 1;
                                END
                            ELSE
                                RAISERROR ('INSERTING Username', 0, 1) WITH NOWAIT;
                                BEGIN
                                    INSERT INTO
                                        azure_temp.GeneratedUsers(
                                            Username
                                        )
                                    VALUES (
                                        @AzureParentUsername
                                    );
                                END
                                RAISERROR ('INSERTED Username', 0, 1) WITH NOWAIT;
                            --SET @RESULT = @AzureParentUsername;
                            SET @RESULT = NULL;
                            RAISERROR ('RESULT: %s', 0, 1, @RESULT) WITH NOWAIT;
                        END
                END TRY
            BEGIN CATCH
                SELECT @ERRORS = ERROR_MESSAGE();
                RETURN 1;
            END CATCH
            RETURN 0;
            END;
            `,
            undefined,
        );
        await queryRunner.query(
            /*We are doing this because we dont know the initial name of the DEFAULT CONSTRAINT. In future migrations for this column it is not needed.*/
            `
            BEGIN
                DECLARE @DefaultConstraintName VARCHAR(100)= '';
                SELECT
                    @DefaultConstraintName = default_constraints.name
                FROM 
                    sys.all_columns
                
                        INNER JOIN
                    sys.tables
                        ON all_columns.object_id = tables.object_id
                
                        INNER JOIN 
                    sys.schemas
                        ON tables.schema_id = schemas.schema_id
                
                        INNER JOIN
                    sys.default_constraints
                        ON all_columns.default_object_id = default_constraints.object_id
                WHERE
                        schemas.name = 'azure_temp'
                    AND tables.name = 'Users'
                    AND all_columns.name = 'Status'
                
                
                EXEC('ALTER TABLE [azure_temp].[Users] DROP CONSTRAINT ' + @DefaultConstraintName);
            END
            `,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Users ADD CONSTRAINT AzureUsersStatusDefault DEFAULT ${EventStatus.IN_USERNAME_GENERATION} FOR Status;`,
            undefined,
        );
        await queryRunner.query(
            `
            CREATE TABLE azure_temp.GeneratedUsers (
                RowID int IDENTITY(1,1) NOT NULL,
                Username varchar(255) NOT NULL,
                Occurrences varchar(255) CONSTRAINT AzureGeneratedUsersOccurrences DEFAULT  1,
                CreatedOn datetime2(7) CONSTRAINT AzureGeneratedUsersCreatedOn DEFAULT  GETUTCDATE() NOT NULL
            )
            `,
            undefined,
        );

        await queryRunner.query(
            `
            CREATE FUNCTION [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@string nvarchar(MAX))
                RETURNS TABLE
                AS RETURN(
                SELECT
                    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@string, N'ый', N'y'), N'ЫЙ', N'Y'), N'а', N'a'), N'б', N'b'), N'в', N'v'), N'г', N'g'), N'д', N'd'), N'е', N'e'), N'ё', N'yo'), N'ж', N'zh'), N'з', N'z'), N'и', N'i'), N'й', N'i'), N'к', N'k'), N'л', N'l'), N'м', N'm'), N'н', N'n'), N'о', N'o'), N'п', N'p'), N'р', N'r'), N'с', N's'), N'т', N't'), N'у', N'u'), N'ф', N'f'), N'х', N'h'), N'ц', N'ts'), N'ч', N'ch'), N'ш', N'sh'), N'щ', N'shch'), N'ъ', N'a'), N'ы', N'yi'), N'ь', N'i'), N'э', N'e'), N'ю', N'iu'), N'я', N'ia'), N'А', N'A'), N'Б', N'B'), N'В', N'V'), N'Г', N'G'), N'Д', N'D'), N'Е', N'E'), N'Ё', N'YO'), N'Ж', N'ZH'), N'З', N'Z'), N'И', N'I'), N'Й', N'I'), N'К', N'K'), N'Л', N'L'), N'М', N'M'), N'Н', N'N'), N'О', N'O'), N'П', N'P'), N'Р', N'R'), N'С', N'S'), N'Т', N'T'), N'У', N'U'), N'Ф', N'F'), N'Х', N'H'), N'Ц', N'TS'), N'Ч', N'CH'), N'Ш', N'SH'), N'Щ', N'SHCH'), N'Ъ', N'A'), N'Ы', N'YE'), N'Ь', N'I'), N'Э', N'E'), N'Ю', N'IU'), N'Я', N'IA') AS RESULT
                );
            `,
            undefined,
        );

        await queryRunner.query(
            `
            CREATE TRIGGER azure_temp.AfterInsert
            ON azure_temp.Users
            AFTER INSERT
            AS
            BEGIN
                --declare variables
                DECLARE @RowID INT;
                DECLARE @FirstName VARCHAR(100);
                DECLARE @MiddleName VARCHAR(100);
                DECLARE @Surname VARCHAR(100);
                DECLARE @Email VARCHAR(100);
                DECLARE @UserRole VARCHAR(100);
                DECLARE @PersonID INT;
                DECLARE @WorkflowType VARCHAR(100);
                BEGIN
                    --fill variables
                    SELECT
                        @RowID = i.RowID,
                        @FirstName = i.FirstName,
                        @MiddleName = i.MiddleName,
                        @Surname = i.Surname,
                        @Email = i.Email,
                        @UserRole = i.UserRole,
                        @PersonID = i.PersonID,
                        @WorkflowType = i.WorkflowType
                    FROM
                        INSERTED i
                END
                BEGIN
                    
                    SET NOCOUNT ON
                    DECLARE @RETURNVALUE int;
                    DECLARE @RESULTOUT varchar(100) = '';
                    DECLARE @ERRORSOUT varchar(100) = '';
                
                    IF @WorkflowType = 'USER_CREATE'
                        BEGIN
                            --call generate function
                            SELECT @FirstName = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@FirstName);
                            SELECT @MiddleName = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@MiddleName);
                            SELECT @Surname = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@Surname);
                            EXEC @RETURNVALUE = azure_temp.GENERATE_USERNAME 
                                @FirstName = @FirstName,
                                @MiddleName = @MiddleName,
                                @LastName = @Surname, 
                                @Email = @Email, 
                                @AzureUserRole = @UserRole, 
                                @PersonID = @PersonID, 
                                @ERRORS = @ERRORSOUT OUTPUT,
                                @RESULT = @RESULTOUT OUTPUT;
                            IF LEN(@ERRORSOUT) > 0
                                BEGIN
                                    --this will set the status to error and set the error message
                                    UPDATE
                                        azure_temp.Users
                                    SET
                                        Status = ${EventStatus.FAILED_USERNAME_GENERATION},
                                        ErrorMessage = @ERRORSOUT
                                    WHERE RowID = @RowID;
                                END
                            ELSE
                                BEGIN
                                    --if all is good the username should be generated and the status should be set to 0 so the create job can pick up the row.
                                    UPDATE
                                        azure_temp.Users
                                    SET
                                        Status = ${EventStatus.AWAITING_CREATION},
                                        Username = @RESULTOUT
                                    WHERE RowID = @RowID;
                                END
                        END
                    ELSE
                        BEGIN
                            --if all is good the username should be generated and the status should be set to 0 so the create job can pick up the row.
                            UPDATE
                                azure_temp.Users
                            SET
                                Status = ${EventStatus.AWAITING_CREATION},
                                Username = NULL
                            WHERE RowID = @RowID;
                        END
                END;
            END
            `,
            undefined,
        );

        await queryRunner.query(
            `
            INSERT
            INTO
                azure_temp.GeneratedUsers (Username)
            SELECT
                DISTINCT SUBSTRING(su.Username, 0, CHARINDEX('@', su.Username, 0) )
            FROM
                core.SysUser su
            JOIN core.EducationalState es ON
                su.PersonID = es.PersonID
            JOIN core.[Position] p on
                es.PositionID = p.PositionID
            LEFT JOIN azure_temp.GeneratedUsers gu  ON gu.Username = 
                SUBSTRING(su.Username, 0, CHARINDEX('@', su.Username, 0) )
            WHERE
                p.SysRoleID IN ( ${RoleEnum.STUDENT} , ${RoleEnum.TEACHER})
                AND su.Username IS NOT NULL
                AND su.Username <> ''
                AND su.DeletedON IS NOT NULL
                AND su.IsAzureUser = ${IsAzureUser.YES}
                AND gu.Username IS NULL
            `,
            undefined,
        );

        await queryRunner.query(
            `
            INSERT
            INTO
                azure_temp.GeneratedUsers (Username)
            SELECT
                DISTINCT u.Email
            FROM
                core.SysUser su
            JOIN azure_temp.Users u ON
                su.PersonID = u.UserID AND u.WorkflowType = '${WorkflowType.USER_CREATE}' AND u.UserRole = '${UserRoleType.PARENT}' AND u.Status <> ${EventStatus.FAILED_USERNAME_GENERATION}
            WHERE
            su.Username IS NOT NULL
                AND su.Username <> ''
                AND su.IsAzureUser = ${IsAzureUser.YES};
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`DROP PROCEDURE azure_temp.GENERATE_USERNAME;`, undefined);
        await queryRunner.query(`DROP TRIGGER azure_temp.AfterInsert`, undefined);
        await queryRunner.query(`DROP FUNCTION [azure_temp].TRANSLATE_CYRILIC_TO_LATIN`, undefined);
        await queryRunner.query(`ALTER TABLE [azure_temp].[Users] DROP CONSTRAINT AzureUsersStatusDefault`, undefined);
        await queryRunner.query(
            `ALTER TABLE azure_temp.Users ADD CONSTRAINT AzureUsersStatusDefault DEFAULT  ${EventStatus.AWAITING_CREATION} FOR Status;`,
            undefined,
        );
        await queryRunner.query(`DROP TABLE azure_temp.GeneratedUsers;`, undefined);
    }
}
