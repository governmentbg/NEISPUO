/* eslint-disable no-empty-function */
/* eslint-disable @typescript-eslint/naming-convention */
import { MigrationInterface, QueryRunner } from 'typeorm';

export class fixUsernamesInUsersTable1653467087882 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `  
            CREATE PROCEDURE azure_temp.TEMP_PROCEDURE_FOR_USERNAME_REGERENERATION
            (
                @RowID INT,
                @FirstName VARCHAR(100),
                @MiddleName VARCHAR(100),
                @Surname VARCHAR(100),
                @Email VARCHAR(100),
                @UserRole VARCHAR(100),
                @PersonID INT,
                @WorkflowType VARCHAR(100)
            )
            AS
            BEGIN
            --declare variables 

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
                                        ErrorMessage = @ERRORSOUT
                                    OUTPUT 
                                        INSERTED.Status as status
                                    WHERE RowID = @RowID;
                                END
                            ELSE
                                BEGIN
                                    --if all is good the username should be generated and the status should be set to 0 so the create job can pick up the row.
                                    UPDATE
                                        azure_temp.Users
                                    SET
                                        Status = 0,
                                        RetryAttempts = 0,
                                        ErrorMessage = NULL,
                                        GUID = newid(),
                                        InProcessing = 0,
                                        UpdatedOn = getutcdate(),
                                        Username = @RESULTOUT
                                    OUTPUT
                                        INSERTED.Status as status
                                    WHERE RowID = @RowID;
                                END
                        END
                    ELSE
                        BEGIN
                            --if all is good the username should be generated and the status should be set to 0 so the create job can pick up the row.
                            UPDATE
                                azure_temp.Users
                            SET
                                Username = NULL
                            OUTPUT
                                INSERTED.Status as status
                            WHERE RowID = @RowID;
                        END
                END;
            END
            `,
            undefined,
        );
        await queryRunner.query(
            `
            BEGIN
                DECLARE @RowID INT;
                DECLARE @FirstName VARCHAR(100);
                DECLARE @MiddleName VARCHAR(100);
                DECLARE @LastName VARCHAR(100);
                DECLARE @Surname VARCHAR(100);
                DECLARE @Email VARCHAR(100);
                DECLARE @UserRole VARCHAR(100);
                DECLARE @PersonID INT;
                DECLARE @WorkflowType VARCHAR(100);
                
                -- Iterate over all customers
                WHILE (1 = 1)
                BEGIN
                    DECLARE @LoopShouldStop INT = 0;
                -- Get next customerId
                    SELECT TOP 
                    1 
                    @RowID = RowID,
                    @FirstName = FirstName,
                    @MiddleName = MiddleName,
                    @LastName = Surname, 
                    @Email = Email, 
                    @UserRole = UserRole, 
                    @PersonID = u.PersonID, 
                    @WorkflowType = WorkflowType,
                    @LoopShouldStop = 1
                    FROM azure_temp.Users u 
                    LEFT JOIN 
                        core.SysUser su on u.PersonID = su.PersonID 
                    WHERE 
                        1 = 1
                        AND u.Status = 2
                        AND u.RetryAttempts = 5
                        AND u.WorkflowType = 'USER_CREATE'
                        AND su.SysUserID IS NULL
                        AND ErrorMessage LIKE '%{"status":200,"statusText":"OK","headers":{"content-type":"application/json; charset=%'
                    ORDER BY u.RowID
                    
                    IF @LoopShouldStop = 0
                    BEGIN;
                        BREAK;
                    END;
                    SELECT @FirstName = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@FirstName);
                    SELECT @MiddleName = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@MiddleName);
                    SELECT @Surname = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@LastName);
                    EXEC azure_temp.TEMP_PROCEDURE_FOR_USERNAME_REGERENERATION
                        @RowID = @RowID, 
                        @FirstName = @FirstName,
                        @MiddleName = @MiddleName,
                        @Surname = @Surname, 
                        @Email = @Email, 
                        @UserRole = @UserRole, 
                        @PersonID = @PersonID, 
                        @WorkflowType = @WorkflowType
                END
            END
            `,
            undefined,
        );
        await queryRunner.query(
            `
            DROP PROCEDURE azure_temp.TEMP_PROCEDURE_FOR_USERNAME_REGERENERATION
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {}
}
